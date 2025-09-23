using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    string CreateToken(Guid userId, string userEmail, UserRole role);
}
