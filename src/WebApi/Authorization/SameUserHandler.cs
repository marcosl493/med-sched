using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Authorization;

public class SameUserRequirement : IAuthorizationRequirement { }

public class SameUserHandler : AuthorizationHandler<SameUserRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, SameUserRequirement requirement)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(claim => claim.Properties.Values.Contains(JwtRegisteredClaimNames.Sub));
        if (userIdClaim is null) return Task.CompletedTask;

        var httpContext = (context.Resource as HttpContext)!;
        if (httpContext.Request.RouteValues.TryGetValue("id", out var routeIdObj) &&
            Guid.TryParse(routeIdObj?.ToString(), out var routeId))
        {
            if (Guid.Parse(userIdClaim.Value) == routeId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

