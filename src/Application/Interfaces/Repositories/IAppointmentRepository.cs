using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IAppointmentRepository
{
    Task CreateAppointmentAsync(Appointment appointment, CancellationToken cancellationToken);
    Task<bool> IsAvaliableAppointmentAsync(Guid scheduleId, CancellationToken cancellationToken);
    Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancellationToken);
}
