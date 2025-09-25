using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IScheduleRepository
{
    Task CreateScheduleAsync(Schedule schedule, CancellationToken cancellationToken);
    Task<Schedule?> GetScheduleByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsAvailableScheduleByPhysicianIdAsync(Guid physicianId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}
