using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.Appointment;
using Moq;
namespace ApplicationTests.UseCases.Appointment;

[TestClass]
public class CreateAppointmentHandlerTests
{
    private Mock<IAppointmentRepository> _appointmentRepositoryMock = null!;
    private Mock<IPatientRepository> _patientRepositoryMock = null!;
    private Mock<IPublisherEvent> _publisherMock = null!;
    private Mock<IScheduleRepository> _scheduleRepositoryMock = null!;
    private CreateAppointmentHandler _handler = null!;
    private Guid _patientId;
    private Guid _scheduleId;
    private string _reason;

    [TestInitialize]
    public void Setup()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _publisherMock = new Mock<IPublisherEvent>();
        _publisherMock
            .Setup(p => p.ProduceEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _scheduleRepositoryMock = new Mock<IScheduleRepository>();

        _handler = new CreateAppointmentHandler(_appointmentRepositoryMock.Object, _patientRepositoryMock.Object, _publisherMock.Object, _scheduleRepositoryMock.Object);
        _patientId = Guid.NewGuid();
        _scheduleId = Guid.NewGuid();
        _reason = "Consulta de rotina";
    }

    [TestMethod]
    public async Task Handle_ReturnsFail_WhenPatientNotFound()
    {
        _patientRepositoryMock
            .Setup(repo => repo.GetPatientByIdAsync(_patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Patient?)null);

        var command = new CreateAppointmentCommand(_patientId, _scheduleId, _reason);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Patient not found.", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Handle_ReturnsFail_WhenAppointmentNotAvailable()
    {
        var patient = new Domain.Entities.Patient();
        _patientRepositoryMock
            .Setup(repo => repo.GetPatientByIdAsync(_patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // create a schedule with a scheduled appointment to make it not available
        var appointment = new Domain.Entities.Appointment(_reason, _scheduleId, _patientId);
        var physician = new Domain.Entities.Physician();
        var schedule = new Domain.Entities.Schedule(_scheduleId, new List<Domain.Entities.Appointment> { appointment }, physician, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        _scheduleRepositoryMock
            .Setup(repo => repo.GetScheduleByIdAsync(_scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        var command = new CreateAppointmentCommand(_patientId, _scheduleId, _reason);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Invalid Schedule.", result.Errors[0].Message);
        Assert.AreEqual(409, result.Errors[0].Metadata["StatusCode"]);
    }

    [TestMethod]
    public async Task Handle_ReturnsSuccess_WhenAppointmentCreated()
    {
        var patient = new Mock<Domain.Entities.Patient>();
        var appointment = new Domain.Entities.Appointment(_reason, _scheduleId, _patientId);

        var user = new Domain.Entities.User("Paciente Teste", "patient@example.com", Domain.Entities.UserRole.PATIENT, "password");
        patient.SetupGet(p => p.User).Returns(user);

        patient
            .Setup(p => p.ScheduleAppointment(_scheduleId, _reason))
            .Returns(appointment);

        _patientRepositoryMock
            .Setup(repo => repo.GetPatientByIdAsync(_patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient.Object);

        // schedule available
        var physician = new Domain.Entities.Physician();
        var schedule = new Domain.Entities.Schedule(_scheduleId, new List<Domain.Entities.Appointment>(), physician, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        _scheduleRepositoryMock
            .Setup(repo => repo.GetScheduleByIdAsync(_scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _appointmentRepositoryMock
            .Setup(repo => repo.CreateAppointmentAsync(appointment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CreateAppointmentCommand(_patientId, _scheduleId, _reason);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsInstanceOfType<CreateAppointmentResponse>(result.Value);
        Assert.AreEqual(appointment.Id, result.Value.Id);
        Assert.AreEqual(appointment.PatientId, result.Value.PatientId);
        Assert.AreEqual(appointment.CreatedAt, result.Value.CreatedAt);
    }
}
