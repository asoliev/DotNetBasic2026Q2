using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using BrainstormSessions.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using BrainstormSessions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    var emailSection = context.Configuration.GetSection("Serilog:Email");
    if (bool.TryParse(emailSection["Enabled"], out bool enabled) && enabled)
    {
        string pickupDirectorySetting = emailSection["PickupDirectory"] ?? "logs/email-pickup";
        string pickupDirectory = Path.IsPathRooted(pickupDirectorySetting)
            ? pickupDirectorySetting
            : Path.GetFullPath(Path.Combine(context.HostingEnvironment.ContentRootPath, pickupDirectorySetting));

        loggerConfiguration.WriteTo.Sink(
            new PickupDirectoryEmailSink(
                from: emailSection["From"]!,
                to: emailSection["To"]!,
                pickupDirectory: pickupDirectory,
                subject: emailSection["Subject"] ?? "BrainstormSessions error log",
                bodyTemplate: emailSection["Body"] ?? "{Timestamp} [{Level}] {Message}{NewLine}{Exception}"),
            restrictedToMinimumLevel: LogEventLevel.Error);
    }
});

builder.Services.AddDbContext<AppDbContext>(
    optionsBuilder => optionsBuilder.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBrainstormSessionRepository, EFStormSessionRepository>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    IBrainstormSessionRepository repository = scope.ServiceProvider.GetRequiredService<IBrainstormSessionRepository>();

    await SeedDatabaseAsync(repository);
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

Log.CloseAndFlush();

static async Task SeedDatabaseAsync(IBrainstormSessionRepository repo)
{
    List<BrainstormSession> sessionList = await repo.ListAsync();
    if (sessionList.Count == 0)
    {
        await repo.AddAsync(GetTestSession());
    }
}

static BrainstormSession GetTestSession()
{
    BrainstormSession session = new()
    {
        Name = "Test Session 1",
        DateCreated = new DateTime(2016, 8, 1)
    };

    Idea idea = new()
    {
        DateCreated = new DateTime(2016, 8, 1),
        Description = "Totally awesome idea",
        Name = "Awesome idea"
    };

    session.AddIdea(idea);
    return session;
}
