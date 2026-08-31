using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Events;
using FluentResults;
using MediatR;
using System.Text.Json;
using System.Transactions;

namespace Application.UseCases.Appointment;

public class CreateAppointmentHandler
    (
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IPublisherEvent publisher,
        IScheduleRepository scheduleRepository
    ) : IRequestHandler<CreateAppointmentCommand, Result<CreateAppointmentResponse>>
{
    private static readonly TransactionOptions transactionOptions = new()
    {
        IsolationLevel = IsolationLevel.ReadCommitted,
        Timeout = TransactionManager.DefaultTimeout
    };
    public async Task<Result<CreateAppointmentResponse>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        const string topic = "appointment-created";
        var patient = await patientRepository.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result.Fail(new Error("Patient not found."));

        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);

        var schedule = await scheduleRepository.GetScheduleByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null || !schedule.IsAvaliableSchedule())
            return Result.Fail(new Error("Invalid Schedule.")
                .WithMetadata("StatusCode", 409));

        var appointment = patient.ScheduleAppointment(request.ScheduleId, request.Reason);
        await appointmentRepository.CreateAppointmentAsync(appointment, cancellationToken);

        var appointmentCreatedEvent = new AppointmentCreatedEvent
            (
                appointment.Id,
                appointment.Reason,
                appointment.Status,
                appointment.PatientId,
                new UserDto(patient.UserId, patient.User.Name, patient.User.Email),
                new ScheduleDto(schedule.Id, schedule.StartTime, schedule.EndTime),
                appointment.CreatedAt
            );
        await publisher.ProduceEventAsync(topic, appointment.Id.ToString(), JsonSerializer.Serialize(appointmentCreatedEvent), cancellationToken);
        scope.Complete();

        var response = new CreateAppointmentResponse(appointment.Id, appointment.PatientId, appointment.CreatedAt);
        return response;
    }
}

public record CreateAppointmentCommand(Guid PatientId, Guid ScheduleId, string Reason) : IRequest<Result<CreateAppointmentResponse>>;

public record CreateAppointmentResponse(Guid Id, Guid PatientId, DateTimeOffset CreatedAt);