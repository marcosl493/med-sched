using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AppointmentRepository(MedSchedDbContext context) : IAppointmentRepository
{
    public async Task CreateAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        await context.Appointments.AddAsync(appointment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancellationToken)
        => context.Appointments
                  .Include(appointment => appointment.Schedule)
                  .SingleOrDefaultAsync(appointment => appointment.Id == appointmentId, cancellationToken);

    public async Task<bool> IsAvaliableAppointmentAsync(Guid scheduleId, CancellationToken cancellationToken)
        => !(await context.Appointments.AnyAsync(appointment => appointment.ScheduleId == scheduleId && appointment.Status == AppointmentStatus.SCHEDULED, cancellationToken));
}
