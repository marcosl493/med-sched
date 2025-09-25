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

    public Task<Schedule?> GetScheduleByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Schedules
                  .Include(sched => sched.Physician)
                    .ThenInclude(physician => physician.User)
                  .Where(sched => sched.Id == id)
                  .Select(sched => new Schedule(sched.Id, sched.Physician, sched.CreatedAt, sched.StartTime, sched.EndTime))
                  .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> IsAvailableScheduleByPhysicianIdAsync(Guid physicianId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)

        => context.Schedules
                  .AnyAsync(sched => sched.PhysicianId == physicianId &&
                                     ((sched.StartTime < endTime && sched.EndTime > startTime) ||
                                      (sched.StartTime >= startTime && sched.EndTime <= endTime)),
                              cancellationToken);
}
