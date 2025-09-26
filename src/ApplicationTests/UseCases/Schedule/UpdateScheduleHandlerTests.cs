using Application.Interfaces.Repositories;
using Application.UseCases.Schedule;
using Moq;

namespace ApplicationTests.UseCases.Schedule;

[TestClass]
public class UpdateScheduleHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;
    private readonly UpdateScheduleHandler _handler;

    public UpdateScheduleHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
        _handler = new UpdateScheduleHandler(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldReturnNotFound_WhenScheduleDoesNotExist()
    {
        // Arrange
        var command = new UpdateScheduleCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now.AddHours(1));
        _repositoryMock.Setup(r => r.GetScheduleByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Schedule?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Recurso não encontrado.", result.Errors[0].Message);
        Assert.AreEqual(404, result.Errors[0].Metadata["StatusCode"]);
    }

    [TestMethod]
    public async Task Handle_ShouldReturnForbidden_WhenPhysicianIdDoesNotMatch()
    {
        // Arrange
        var schedule = new Domain.Entities.Schedule(Guid.NewGuid(), DateTime.Now.AddMinutes(5), DateTime.Now.AddMinutes(35));
        var command = new UpdateScheduleCommand(schedule.Id, Guid.NewGuid(), DateTime.Now, DateTime.Now.AddHours(1));
        _repositoryMock.Setup(r => r.GetScheduleByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Recurso não autorizado", result.Errors[0].Message);
        Assert.AreEqual(403, result.Errors[0].Metadata["StatusCode"]);
    }

    [TestMethod]
    public async Task Handle_ShouldUpdateSchedule_WhenValidRequest()
    {
        // Arrange
        var physicianId = Guid.NewGuid();
        var schedule = new Domain.Entities.Schedule(physicianId, DateTime.Now.AddMinutes(5), DateTime.Now.AddMinutes(35));
        var command = new UpdateScheduleCommand(schedule.Id, physicianId, DateTime.Now, DateTime.Now.AddHours(1));
        _repositoryMock.Setup(r => r.GetScheduleByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _repositoryMock.Setup(r => r.UpdateScheduleAsync(schedule, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateScheduleAsync(schedule, It.IsAny<CancellationToken>()), Times.Once);
    }
}
