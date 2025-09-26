using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ScheduleRepository(MedSchedDbContext context) : IScheduleRepository
{
    public async Task CreateScheduleAsync(Schedule schedule, CancellationToken cancellationToken)
    {
        await context.Schedules.AddAsync(schedule, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Schedule[]> GetAllAvaliableScheduleAsync(Guid? physicianId, DateTime? startTime, int top, int? skip, CancellationToken cancellationToken)
    {
        var query = context.Schedules
                            .Include(sched => sched.Physician)
                                .ThenInclude(physician => physician.User)
                            .Include(sched => sched.Appointments)
                            .Where(sched => sched.StartTime > (startTime ?? DateTimeOffset.UtcNow)
                                    && !sched.Appointments
                                            .Any(appointment => appointment.Status == AppointmentStatus.SCHEDULED));

        if (physicianId.HasValue)
            query = query.Where(sched => sched.PhysicianId == physicianId);
        if (skip.HasValue)
            query = query.Skip(skip.Value);

        return query
            .Take(top)
            .Select(sched => new Schedule(sched.Id, sched.Appointments, sched.Physician, sched.CreatedAt, sched.StartTime, sched.EndTime))
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<Schedule?> GetScheduleByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Schedules
                  .Include(sched => sched.Physician)
                    .ThenInclude(physician => physician.User)
                  .Include(sched => sched.Appointments)
                  .Where(sched => sched.Id == id)
                  .Select(sched => new Schedule(sched.Id, sched.Appointments, sched.Physician, sched.CreatedAt, sched.StartTime, sched.EndTime))
                  .AsNoTracking()
                  .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> IsAvailableScheduleByPhysicianIdAsync(Guid physicianId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)

        => context.Schedules
                  .AnyAsync(sched => sched.PhysicianId == physicianId &&
                                     ((sched.StartTime < endTime && sched.EndTime > startTime) ||
                                      (sched.StartTime >= startTime && sched.EndTime <= endTime)),
                              cancellationToken);
    public Task UpdateScheduleAsync(Schedule schedule, CancellationToken cancellationToken)
    {
        context.Schedules.Update(schedule);
        return context.SaveChangesAsync(cancellationToken);
    }
}
