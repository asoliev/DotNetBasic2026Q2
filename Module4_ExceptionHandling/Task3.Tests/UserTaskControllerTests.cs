using System;
using NUnit.Framework;
using Task3.DoNotChange;
using Task3.Tests.Stubs;

namespace Task3.Tests;

[TestFixture]
public class UserTaskControllerTests
{
    private readonly UserTaskController _controller;
    private readonly IUserDao _userDao;

    public UserTaskControllerTests()
    {
        _userDao = new UserDaoStub();
        var taskService = new UserTaskService(_userDao);
        _controller = new UserTaskController(taskService);
    }

    [Test]
    public void CreateUserTask_ValidData_ReturnsTaskAndEmptyMessage()
    {
        var model = new ResponseModelStub();
        const string description = "task4";
        const int userId = 1;

        bool result = _controller.AddTaskForUser(userId, description, model);

        Assert.That(result, Is.EqualTo(true));
        Assert.That(model.GetActionResult(), Is.Null);
        Assert.That(_userDao.GetUser(userId).Tasks.Count, Is.EqualTo(4));
        Assert.That(_userDao.GetUser(userId).Tasks[3].Description, Is.EqualTo(description).IgnoreCase);
    }

    [Test]
    public void CreateUserTask_InvalidUserId_ReturnsNullAndInvalidUserIdMessage()
    {
        var model = new ResponseModelStub();
        const string description = "task4";
        const int userId = -11;
        const int existingUserId = 1;

        bool result = _controller.AddTaskForUser(userId, description, model);

        Assert.That(result, Is.EqualTo(false));
        Assert.That(model.GetActionResult(), Is.EqualTo("Invalid userId").IgnoreCase);
        Assert.That(_userDao.GetUser(existingUserId).Tasks.Count, Is.EqualTo(3));
    }

    [Test]
    public void CreateUserTask_NonExistentUser_ReturnsNullAndUserNotFoundMessage()
    {
        var model = new ResponseModelStub();
        const string description = "task4";
        const int userId = 2;
        const int existingUserId = 1;

        bool result = _controller.AddTaskForUser(userId, description, model);

        Assert.That(result, Is.EqualTo(false));
        Assert.That(model.GetActionResult(), Is.EqualTo("User not found").IgnoreCase);
        Assert.That(_userDao.GetUser(existingUserId).Tasks.Count, Is.EqualTo(3));
    }

    [Test]
    public void CreateUserTask_TaskAlreadyExists_ReturnsNullAndTheTaskAlreadyExistsMessage()
    {
        var model = new ResponseModelStub();
        const string description = "task3";
        const int userId = 1;

        bool result = _controller.AddTaskForUser(userId, description, model);

        Assert.That(result, Is.EqualTo(false));
        Assert.That(model.GetActionResult(), Is.EqualTo("The task already exists").IgnoreCase);
        Assert.That(_userDao.GetUser(userId).Tasks.Count, Is.EqualTo(3));
    }

    [Test]
    public void CreateUserTask_FutureUserTaskExceptionSubtype_IsHandledByBaseCatch()
    {
        const string expectedMessage = "Future domain rule failed";
        var controller = new UserTaskController(new UserTaskService(new ThrowingUserDao(expectedMessage)));
        var model = new ResponseModelStub();

        bool result = controller.AddTaskForUser(1, "task-new", model);

        Assert.That(result, Is.False);
        Assert.That(model.GetActionResult(), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void CreateUserTask_UnexpectedException_IsHandledByGeneralCatch()
    {
        const string unexpectedError = "Database connection failed";
        var controller = new UserTaskController(new UserTaskService(new ThrowingUserDaoUnexpected(unexpectedError)));
        var model = new ResponseModelStub();

        bool result = controller.AddTaskForUser(1, "task-new", model);

        Assert.That(result, Is.False);
        Assert.That(model.GetActionResult(), Does.Contain("An unexpected error occurred"));
        Assert.That(model.GetActionResult(), Does.Contain(unexpectedError));
    }

    private sealed class FutureUserTaskException(string message) : UserTaskException(message);

    private sealed class ThrowingUserDao(string message) : IUserDao
    {
        public IUser GetUser(int id) => throw new FutureUserTaskException(message);
    }

    private sealed class ThrowingUserDaoUnexpected(string message) : IUserDao
    {
        public IUser GetUser(int id) => throw new InvalidOperationException(message);
    }
}
