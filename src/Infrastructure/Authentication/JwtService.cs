using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication;

public class JwtService(IOptionsSnapshot<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions options = options.Value;
    public (string AccessToken, string TokenType, ushort ExpiresIn) CreateToken(Guid userId, string userEmail, UserRole role)
    {
        var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresInMinute = DateTime.UtcNow.AddMinutes(options.ExpiresInMinutes);
        var token = new JwtSecurityToken(
           issuer: options.Issuer,
           audience: options.Audience,
           claims: claims,
           expires: expiresInMinute,
           signingCredentials: creds
       );

        return (new JwtSecurityTokenHandler()
            .WriteToken(token), "Bearer", (ushort)options.ExpiresInMinutes);
    }
}
