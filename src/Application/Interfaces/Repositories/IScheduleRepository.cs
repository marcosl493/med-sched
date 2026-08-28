using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IScheduleRepository
{
    Task CreateScheduleAsync(Schedule schedule, CancellationToken cancellationToken);
    Task<Schedule?> GetScheduleByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsAvailableScheduleByPhysicianIdAsync(Guid physicianId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken);
    Task<(Schedule[] Schedules, int Count)> GetAllScheduleAsync(Guid? physicianId, DateTimeOffset? startTime, bool? onlyAvaliable, int top, int? skip, CancellationToken cancellationToken);
    Task UpdateScheduleAsync(Schedule schedule, CancellationToken cancellationToken);
    Task<bool> IsAvaliableToDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
