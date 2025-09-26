using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PatientRepository(MedSchedDbContext context) : IPatientRepository
{
    public async Task CreatePatientAsync(Patient patient, CancellationToken cancellationToken)
    {
        await context.Patients.AddAsync(patient, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context
            .Patients
            .Include(patient => patient.User)
            .SingleOrDefaultAsync(patient => patient.Id == id, cancellationToken);
    }
    public async Task<Guid?> GetPatientIdByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = await context
            .Patients
            .Include(patient => patient.User)
            .Select(patient => new { patient.Id, patient.UserId })
            .AsNoTracking()
            .SingleOrDefaultAsync(patient => patient.UserId == userId, cancellationToken);
        return query?.Id;
    }

    public Task UpdatePatientAsync(Patient patient, CancellationToken cancellationToken)
    { 
        context.Patients.Update(patient);
        return context.SaveChangesAsync(cancellationToken);
    }
}
