using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(MedSchedDbContext context) : IUserRepository
{
    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        => context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
}
