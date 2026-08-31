using Domain.Entities;

namespace Domain.Events;

public sealed record AppointmentCreatedEvent(
    Guid Id,
    string Reason,
    AppointmentStatus Status,
    Guid PatientId,
    UserDto User,
    ScheduleDto Schedule,
    DateTimeOffset CreatedAt
);

public sealed record UserDto
    (
        Guid UserId,
        string Name,
        string Email
    );
public sealed record ScheduleDto
    (
        Guid ScheduleId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime
    );