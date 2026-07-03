using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using BrainstormSessions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainstormSessions.Controllers;

public partial class SessionController(
    IBrainstormSessionRepository sessionRepository,
    ILogger<SessionController> logger) : Controller
{
    public async Task<IActionResult> Index(int? id)
    {
        if (!id.HasValue)
        {
            LogSessionIndexMissingId(logger);
            return RedirectToAction(actionName: nameof(Index),
                controllerName: "Home");
        }

        LogLoadingSession(logger, id.Value);

        BrainstormSession session = await sessionRepository.GetByIdAsync(id.Value);
        if (session == null)
        {
            LogSessionNotFound(logger, id.Value);
            return Content("Session not found.");
        }

        LogLoadedSession(logger, session.Id);

        StormSessionViewModel viewModel = new()
        {
            DateCreated = session.DateCreated,
            Name = session.Name,
            Id = session.Id
        };

        LogPreparedSessionViewModel(logger, session.Id);

        return View(viewModel);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Session index request did not include a session id.")]
    private static partial void LogSessionIndexMissingId(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Loading session with id {SessionId}.")]
    private static partial void LogLoadingSession(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Session with id {SessionId} was not found.")]
    private static partial void LogSessionNotFound(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Prepared session view model for session {SessionId}.")]
    private static partial void LogPreparedSessionViewModel(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded session with id {SessionId}.")]
    private static partial void LogLoadedSession(ILogger logger, int sessionId);
}
