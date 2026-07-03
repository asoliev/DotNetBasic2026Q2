using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using BrainstormSessions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainstormSessions.Controllers;

public partial class HomeController(
    IBrainstormSessionRepository sessionRepository,
    ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        LogLoadingHomePageSessionList(logger);

        List<BrainstormSession> sessionList = await sessionRepository.ListAsync();
        LogLoadedHomePageSessions(logger, sessionList.Count);

        IEnumerable<StormSessionViewModel> model = sessionList.Select(session => new StormSessionViewModel()
        {
            Id = session.Id,
            DateCreated = session.DateCreated,
            Name = session.Name,
            IdeaCount = session.Ideas.Count
        });

        return View(model);
    }

    public class NewSessionModel
    {
        [Required]
        public string SessionName { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Index(NewSessionModel model)
    {
        if (!ModelState.IsValid)
        {
            LogInvalidSessionSubmission(logger);
            return BadRequest(ModelState);
        }

        LogCreatingBrainstormSession(logger, model.SessionName);

        await sessionRepository.AddAsync(new()
        {
            DateCreated = DateTimeOffset.Now,
            Name = model.SessionName
        });

        LogCreatedBrainstormSession(logger, model.SessionName);

        return RedirectToAction(actionName: nameof(Index));
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Loading home page session list.")]
    private static partial void LogLoadingHomePageSessionList(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {SessionCount} brainstorm sessions for the home page.")]
    private static partial void LogLoadedHomePageSessions(ILogger logger, int sessionCount);

    [LoggerMessage(Level = LogLevel.Error, Message = "Invalid session submission received for the home page.")]
    private static partial void LogInvalidSessionSubmission(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating a new brainstorm session named {SessionName}.")]
    private static partial void LogCreatingBrainstormSession(ILogger logger, string sessionName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Created a new brainstorm session named {SessionName}.")]
    private static partial void LogCreatedBrainstormSession(ILogger logger, string sessionName);
}
