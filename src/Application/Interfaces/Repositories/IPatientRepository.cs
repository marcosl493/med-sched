using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPatientRepository
{
    Task CreatePatientAsync(Patient patient, CancellationToken cancellationToken);
    Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid?> GetPatientIdByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdatePatientAsync(Patient patient, CancellationToken cancellationToken);
}
