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

    public async Task<(Appointment[], int Count)> GetAllAppointmentsAsync(int top, Guid? patientId, Guid? physicianId, AppointmentStatus? status, int? skip, CancellationToken cancellationToken)
    {
        var query = context.Appointments
                            .Include(appointment => appointment.Schedule)
                            .Include(appointment => appointment.Patient)
                                .ThenInclude(patient => patient.User)
                            .AsQueryable();
        if (patientId.HasValue)
            query = query.Where(appointment => appointment.PatientId == patientId.Value);
        if (physicianId.HasValue)
            query = query.Where(appointment => appointment.Schedule.PhysicianId == physicianId.Value);
        if (status.HasValue)
            query = query.Where(appointment => appointment.Status == status.Value);

        var count = await query.CountAsync(cancellationToken);
        if (skip.HasValue)
            query = query.Skip(skip.Value);

        return (await query.Take(top).ToArrayAsync(cancellationToken), count);

    }

    public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancellationToken)
        => context.Appointments
                  .Include(appointment => appointment.Schedule)
                  .Include(appointment => appointment.Patient)
                    .ThenInclude(patient => patient.User)
                  .SingleOrDefaultAsync(appointment => appointment.Id == appointmentId, cancellationToken);

    public async Task<bool> IsAvaliableAppointmentAsync(Guid scheduleId, CancellationToken cancellationToken)
        => !(await context.Appointments.AnyAsync(appointment => appointment.ScheduleId == scheduleId && appointment.Status == AppointmentStatus.SCHEDULED, cancellationToken));
}
