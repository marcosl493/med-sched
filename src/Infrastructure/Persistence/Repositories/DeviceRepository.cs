using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class DeviceRepository
    (
        MedSchedDbContext dbContext
    ) : IDeviceRepository
{
    private readonly MedSchedDbContext _dbContext = dbContext;
    public Task<List<Device>> GetDevicesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext
            .Devices
            .Where(d => d.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
