using Domain.Entities;

namespace Domain.Events;

public sealed record AppointmentCreatedEvent(
    Guid Id,
    string Reason,
    AppointmentStatus Status,
    Guid PatientId,
    Guid ScheduleId,
    DateTimeOffset CreatedAt
);
