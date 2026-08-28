using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task CreateAppointmentAsync(Appointment appointment, CancellationToken cancellationToken);
    Task<bool> IsAvaliableAppointmentAsync(Guid scheduleId, CancellationToken cancellationToken);
    Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancellationToken);
    Task<(Appointment[], int Count)> GetAllAppointmentsAsync(int top,
        Guid? patientId,
        Guid? physicianId,
        AppointmentStatus? status,
        int? skip,
        CancellationToken cancellationToken);
}
