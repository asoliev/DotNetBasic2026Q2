using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Api;
using BrainstormSessions.Controllers;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BrainstormSessions.Test.UnitTests;

public class LoggingTests
{
    private static Mock<ILogger<T>> CreateEnabledLoggerMock<T>()
    {
        Mock<ILogger<T>> mockLogger = new();
        mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        return mockLogger;
    }

    [Fact]
    public async Task HomeController_Index_LogInfoMessages()
    {
        // Arrange
        Mock<IBrainstormSessionRepository> mockRepo = new();
        mockRepo.Setup(repo => repo.ListAsync())
            .ReturnsAsync(GetTestSessions());
        Mock<ILogger<HomeController>> mockLogger = CreateEnabledLoggerMock<HomeController>();
        HomeController controller = new(mockRepo.Object, mockLogger.Object);

        // Act
        await controller.Index();

        // Assert
        VerifyLog(mockLogger, LogLevel.Information, Times.AtLeastOnce());
    }

    [Fact]
    public async Task HomeController_IndexPost_LogErrorMessage_WhenModelStateIsInvalid()
    {
        // Arrange
        Mock<IBrainstormSessionRepository> mockRepo = new();
        mockRepo.Setup(repo => repo.ListAsync())
            .ReturnsAsync(GetTestSessions());
        Mock<ILogger<HomeController>> mockLogger = CreateEnabledLoggerMock<HomeController>();
        HomeController controller = new(mockRepo.Object, mockLogger.Object);
        controller.ModelState.AddModelError("SessionName", "Required");
        HomeController.NewSessionModel newSession = new();

        // Act
        await controller.Index(newSession);

        // Assert
        VerifyLog(mockLogger, LogLevel.Error, Times.AtLeastOnce());
    }

    [Fact]
    public async Task IdeasController_CreateActionResult_LogErrorMessage_WhenModelStateIsInvalid()
    {
        // Arrange & Act
        Mock<IBrainstormSessionRepository> mockRepo = new();
        Mock<ILogger<IdeasController>> mockLogger = CreateEnabledLoggerMock<IdeasController>();
        IdeasController controller = new(mockRepo.Object, mockLogger.Object);
        controller.ModelState.AddModelError("error", "some error");

        // Act
        await controller.CreateActionResult(model: null);

        // Assert
        VerifyLog(mockLogger, LogLevel.Error, Times.AtLeastOnce());
    }

    [Fact]
    public async Task SessionController_Index_LogDebugMessages()
    {
        // Arrange
        const int testSessionId = 1;
        Mock<IBrainstormSessionRepository> mockRepo = new();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync(GetTestSessions().FirstOrDefault(
                s => s.Id == testSessionId));
        Mock<ILogger<SessionController>> mockLogger = CreateEnabledLoggerMock<SessionController>();
        SessionController controller = new(mockRepo.Object, mockLogger.Object);

        // Act
        await controller.Index(testSessionId);

        // Assert
        VerifyLog(mockLogger, LogLevel.Debug, Times.Exactly(2));
    }

    private static void VerifyLog<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel, Times times)
    {
        loggerMock.Verify(
            logger => logger.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            times);
    }

    private static List<BrainstormSession> GetTestSessions() =>
    [
        new()
        {
            DateCreated = new DateTime(2016, 7, 2),
            Id = 1,
            Name = "Test One"
        },
        new()
        {
            DateCreated = new DateTime(2016, 7, 1),
            Id = 2,
            Name = "Test Two"
        }
    ];
}
