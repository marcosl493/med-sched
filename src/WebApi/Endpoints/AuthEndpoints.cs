using Application.UseCases.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/auth").WithTags("Auth");
            routeGroupBuilder.MapPost("", Login)
                .WithName("Login")
                .Produces<LoginResult>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithDescription("Autentica um usuário, dado as suas credenciais.");
        }
        private static async Task<IResult> Login(LoginCommand request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.ToHttpResult();
        }
    }
}
