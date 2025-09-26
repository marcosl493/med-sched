using Application.Interfaces.Repositories;
using Application.UseCases.Schedule;
using Moq;

namespace ApplicationTests.UseCases.Schedule;

[TestClass]
public class CreateScheduleHandlerTests
{
    private Mock<IScheduleRepository> _repositoryMock = null!;
    private CreateScheduleHandler _handler = null!;
    private Guid _physicianId;
    private DateTime _startTime;
    private DateTime _endTime;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
        _handler = new CreateScheduleHandler(_repositoryMock.Object);
        _physicianId = Guid.NewGuid();
        _startTime = DateTime.UtcNow.AddHours(1);
        _endTime = _startTime.AddHours(1);
    }

    [TestMethod]
    public async Task Handle_ShouldFail_WhenEndTimeIsNotGreaterThanStartTime()
    {
        var command = new CreateScheduleCommand(_physicianId, _startTime, _startTime);

        var result = await _handler.Handle(command, default);

        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("EndTime must be greater than StartTime", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Handle_ShouldFail_WhenScheduleIsNotAvailable()
    {
        _repositoryMock
            .Setup(r => r.IsAvailableScheduleByPhysicianIdAsync(_physicianId, _startTime, _endTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateScheduleCommand(_physicianId, _startTime, _endTime);

        var result = await _handler.Handle(command, default);

        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("There is already a schedule for this physician in the given time range.", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Handle_ShouldCreateSchedule_WhenValidRequest()
    {
        _repositoryMock
            .Setup(r => r.IsAvailableScheduleByPhysicianIdAsync(_physicianId, _startTime, _endTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.CreateScheduleAsync(It.IsAny<Domain.Entities.Schedule>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CreateScheduleCommand(_physicianId, _startTime, _endTime);

        var result = await _handler.Handle(command, default);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Created", result.Successes[0].Message);
        Assert.AreEqual(_physicianId, result.Value.PhysicianId);
        Assert.AreEqual(_startTime, result.Value.StartTime);
        Assert.AreEqual(_endTime, result.Value.EndTime);
    }
}
