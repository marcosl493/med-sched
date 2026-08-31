using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IDeviceRepository
{
    Task<List<Device>> GetDevicesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
