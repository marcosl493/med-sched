using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PhysicianRepository(MedSchedDbContext context) : IPhysicianRepository
{
    public async Task<Guid?> GetPhysicianIdByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = await context
                .Physicians
                .Include(physician => physician.User)
                .Select(physician => new { physician.Id, physician.UserId })
                .AsNoTracking()
                .SingleOrDefaultAsync(physician => physician.UserId == userId, cancellationToken);
        return query?.Id;
    }
}
