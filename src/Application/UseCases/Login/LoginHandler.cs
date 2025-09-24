using Application.Interfaces;
using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using System.ComponentModel;

namespace Application.UseCases.Login;

internal class LoginHandler(IUserRepository repository, ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.CheckPassword(request.Password))
        {
            return Result.Fail<LoginResult>(new Error("Usuário ou senha inválidas.").WithMetadata("StatusCode", 401));
        }
        var token = tokenService.CreateToken(user.Id, user.Email, user.Role);
        return Result.Ok(new LoginResult(token.AccessToken, token.ExpiresIn, token.TokenType));
    }
}
public record LoginResult([Description("Token de acesso JWT utilizado para autenticação.")] string AccessToken,
    [Description("Tempo de expiração do Token de acesso em Minutos")] ushort ExpiresIn,
    [Description("Tipo de token gerado.")] string TokenType);
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResult>>;
