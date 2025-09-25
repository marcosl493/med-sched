using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using WebApi.Authorization;
using WebApi.Options;

namespace WebApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        using var sp = services.BuildServiceProvider();
        var jwtOptions = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
        services.AddScoped<ITokenService, JwtService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                    };
                });

        services.AddSingleton<IAuthorizationHandler, SameUserHandler>();
        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(UserRole.PHYSICIAN), policy =>
            {
                policy.RequireClaim("iss", jwtOptions.Issuer);
                policy.RequireClaim("aud", jwtOptions.Audience);
                policy.RequireAuthenticatedUser();
                policy.RequireRole(nameof(UserRole.PHYSICIAN));
                policy.AddRequirements(new SameUserRequirement());
            }).AddPolicy(nameof(UserRole.PATIENT), policy =>
            {
                policy.RequireClaim("iss", jwtOptions.Issuer);
                policy.RequireClaim("aud", jwtOptions.Audience);
                policy.RequireAuthenticatedUser();
                policy.RequireRole(nameof(UserRole.PATIENT));
                policy.AddRequirements(new SameUserRequirement());
            });

        services.AddRateLimiter(options =>
        {
            var rateLimitConfig = configuration.GetSection(AuthenticationRateLimitOptions.SectionName)
            .Get<AuthenticationRateLimitOptions>() ?? new AuthenticationRateLimitOptions();
            options.AddFixedWindowLimiter(AuthenticationRateLimitOptions.SectionName, opt =>
            {
                opt.PermitLimit = rateLimitConfig.PermitLimit;
                opt.QueueLimit = rateLimitConfig.QueueLimit;
                opt.Window = TimeSpan.FromSeconds(rateLimitConfig.WindowSeconds);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        return services;
    }
}
