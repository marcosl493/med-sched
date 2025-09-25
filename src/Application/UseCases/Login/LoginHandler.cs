using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentResults;
using MediatR;
using System.ComponentModel;

namespace Application.UseCases.Login;

internal class LoginHandler(IUserRepository repository,
    IPatientRepository patientRepository,
    IPhysicianRepository physicianRepository,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken);
        var userIdRole = user?.Role switch
        {
            UserRole.PATIENT => await patientRepository.GetPatientIdByUserIdAsync(user.Id, cancellationToken),
            UserRole.PHYSICIAN => await physicianRepository.GetPhysicianIdByUserIdAsync(user.Id, cancellationToken),
            _ => Guid.Empty
        };
        if (user is null || userIdRole is null || !user.CheckPassword(request.Password))
        {
            return Result.Fail<LoginResult>(new Error("Usuário ou senha inválidas.").WithMetadata("StatusCode", 401));
        }

        var token = tokenService.CreateToken(userIdRole.Value, user.Email, user.Role);
        return Result.Ok(new LoginResult(token.AccessToken, token.ExpiresIn, token.TokenType));
    }
}

public record LoginResult([Description("Token de acesso JWT utilizado para autenticação.")] string AccessToken,
    [Description("Tempo de expiração do Token de acesso em Minutos")] ushort ExpiresIn,
    [Description("Tipo de token gerado.")] string TokenType);
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResult>>;
