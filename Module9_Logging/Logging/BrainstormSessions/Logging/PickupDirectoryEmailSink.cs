using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace BrainstormSessions.Logging;

internal sealed class PickupDirectoryEmailSink(
    string from,
    string to,
    string pickupDirectory,
    string subject,
    string bodyTemplate) : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        Directory.CreateDirectory(pickupDirectory);

        using MailMessage message = new(from, to)
        {
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Body = RenderBody(logEvent),
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = false
        };

        using SmtpClient client = new()
        {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = pickupDirectory
        };

        client.Send(message);
    }

    private string RenderBody(LogEvent logEvent)
    {
        MessageTemplateTextFormatter formatter = new(bodyTemplate, null);
        using StringWriter bodyWriter = new();
        formatter.Format(logEvent, bodyWriter);
        return bodyWriter.ToString();
    }
}