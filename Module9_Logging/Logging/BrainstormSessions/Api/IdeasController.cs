using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.ClientModels;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainstormSessions.Api;

public partial class IdeasController(
    IBrainstormSessionRepository sessionRepository,
    ILogger<IdeasController> logger) : ControllerBase
{

    #region snippet_ForSessionAndCreate
    [HttpGet("forsession/{sessionId}")]
    public async Task<IActionResult> ForSession(int sessionId)
    {
        LogLoadingIdeasForSession(logger, sessionId);

        BrainstormSession session = await sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            LogIdeasSessionNotFound(logger, sessionId);
            return NotFound(sessionId);
        }

        LogLoadedIdeasForSession(logger, session.Ideas.Count, sessionId);

        List<IdeaDTO> result = [.. session.Ideas.Select(idea => new IdeaDTO()
        {
            Id = idea.Id,
            Name = idea.Name,
            Description = idea.Description,
            DateCreated = idea.DateCreated
        })];

        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody]NewIdeaModel model)
    {
        if (!ModelState.IsValid)
        {
            LogInvalidIdeaSubmission(logger, model?.SessionId ?? 0);
            return BadRequest(ModelState);
        }

        LogCreatingIdeaForSession(logger, model.SessionId);

        BrainstormSession session = await sessionRepository.GetByIdAsync(model.SessionId);
        if (session == null)
        {
            LogCreateIdeaSessionNotFound(logger, model.SessionId);
            return NotFound(model.SessionId);
        }

        Idea idea = new()
        {
            DateCreated = DateTimeOffset.Now,
            Description = model.Description,
            Name = model.Name
        };
        session.AddIdea(idea);

        await sessionRepository.UpdateAsync(session);

        LogCreatedIdea(logger, model.Name, session.Id);

        return Ok(session);
    }
    #endregion

    #region snippet_ForSessionActionResult
    [HttpGet("forsessionactionresult/{sessionId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<IdeaDTO>>> ForSessionActionResult(int sessionId)
    {
        LogLoadingIdeasActionResult(logger, sessionId);

        BrainstormSession session = await sessionRepository.GetByIdAsync(sessionId);

        if (session == null)
        {
            LogIdeasActionResultSessionNotFound(logger, sessionId);
            return NotFound(sessionId);
        }

        LogLoadedIdeasActionResult(logger, sessionId, session.Ideas.Count);

        List<IdeaDTO> result = [.. session.Ideas.Select(idea => new IdeaDTO()
        {
            Id = idea.Id,
            Name = idea.Name,
            Description = idea.Description,
            DateCreated = idea.DateCreated
        })];

        return result;
    }
    #endregion

    #region snippet_CreateActionResult
    [HttpPost("createactionresult")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<BrainstormSession>> CreateActionResult([FromBody]NewIdeaModel model)
    {
        if (!ModelState.IsValid)
        {
            LogInvalidIdeaActionResultSubmission(logger, model?.SessionId ?? 0);
            return BadRequest(ModelState);
        }

        LogCreatingIdeaActionResult(logger, model.SessionId);

        BrainstormSession session = await sessionRepository.GetByIdAsync(model.SessionId);

        if (session == null)
        {
            LogCreateIdeaActionResultSessionNotFound(logger, model.SessionId);
            return NotFound(model.SessionId);
        }

        Idea idea = new()
        {
            DateCreated = DateTimeOffset.Now,
            Description = model.Description,
            Name = model.Name
        };
        session.AddIdea(idea);

        await sessionRepository.UpdateAsync(session);

        LogCreatedIdeaActionResult(logger, model.Name, session.Id);

        return CreatedAtAction(nameof(CreateActionResult), new { id = session.Id }, session);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Loading ideas for session {SessionId}.")]
    private static partial void LogLoadingIdeasForSession(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unable to load ideas because session {SessionId} was not found.")]
    private static partial void LogIdeasSessionNotFound(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {IdeaCount} ideas for session {SessionId}.")]
    private static partial void LogLoadedIdeasForSession(ILogger logger, int ideaCount, int sessionId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Invalid idea submission received for session {SessionId}.")]
    private static partial void LogInvalidIdeaSubmission(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating an idea for session {SessionId}.")]
    private static partial void LogCreatingIdeaForSession(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unable to create idea because session {SessionId} was not found.")]
    private static partial void LogCreateIdeaSessionNotFound(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Created a new idea named {IdeaName} for session {SessionId}.")]
    private static partial void LogCreatedIdea(ILogger logger, string ideaName, int sessionId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Loading ideas action result for session {SessionId}.")]
    private static partial void LogLoadingIdeasActionResult(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unable to load ideas action result because session {SessionId} was not found.")]
    private static partial void LogIdeasActionResultSessionNotFound(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded ideas action result for session {SessionId} with {IdeaCount} ideas.")]
    private static partial void LogLoadedIdeasActionResult(ILogger logger, int sessionId, int ideaCount);

    [LoggerMessage(Level = LogLevel.Error, Message = "Invalid idea action result submission received for session {SessionId}.")]
    private static partial void LogInvalidIdeaActionResultSubmission(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating idea action result for session {SessionId}.")]
    private static partial void LogCreatingIdeaActionResult(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unable to create idea action result because session {SessionId} was not found.")]
    private static partial void LogCreateIdeaActionResultSessionNotFound(ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Created idea action result named {IdeaName} for session {SessionId}.")]
    private static partial void LogCreatedIdeaActionResult(ILogger logger, string ideaName, int sessionId);
    #endregion
}
