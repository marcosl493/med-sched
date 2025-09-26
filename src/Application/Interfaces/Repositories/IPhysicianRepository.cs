namespace Application.Interfaces.Repositories;

public interface IPhysicianRepository
{
    Task<Guid?> GetPhysicianIdByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
