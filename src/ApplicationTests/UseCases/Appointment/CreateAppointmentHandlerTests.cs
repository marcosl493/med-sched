using Application.Interfaces.Repositories;
using Application.UseCases.Appointment;
using Moq;
namespace ApplicationTests.UseCases.Appointment;

[TestClass]
public class CreateAppointmentHandlerTests
{
    private Mock<IAppointmentRepository> _appointmentRepositoryMock = null!;
    private Mock<IPatientRepository> _patientRepositoryMock = null!;
    private CreateAppointmentHandler _handler = null!;
    private Guid _patientId;
    private Guid _scheduleId;
    private string _reason;

    [TestInitialize]
    public void Setup()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _handler = new CreateAppointmentHandler(_appointmentRepositoryMock.Object, _patientRepositoryMock.Object);
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

        _appointmentRepositoryMock
            .Setup(repo => repo.IsAvaliableAppointmentAsync(_scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateAppointmentCommand(_patientId, _scheduleId, _reason);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Invaliable Schedule.", result.Errors[0].Message);
        Assert.AreEqual(409, result.Errors[0].Metadata["StatusCode"]);
    }

    [TestMethod]
    public async Task Handle_ReturnsSuccess_WhenAppointmentCreated()
    {
        var patient = new Mock<Domain.Entities.Patient>();
        var appointment = new Domain.Entities.Appointment(_reason, _scheduleId, _patientId);

        patient
            .Setup(p => p.ScheduleAppointment(_scheduleId, _reason))
            .Returns(appointment);

        _patientRepositoryMock
            .Setup(repo => repo.GetPatientByIdAsync(_patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient.Object);

        _appointmentRepositoryMock
            .Setup(repo => repo.IsAvaliableAppointmentAsync(_scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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
