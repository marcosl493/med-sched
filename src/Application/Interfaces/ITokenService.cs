using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    (string AccessToken, string TokenType, ushort ExpiresIn) CreateToken(Guid userId, string userEmail, UserRole role);
}
