using Application.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<MedSchedDbContext>((sp, options) =>
            options.UseNpgsql
            (
                sp.GetRequiredService<IConfiguration>().BuildConnectionString(nameof(MedSchedDbContext)),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(MedSchedDbContext).Assembly.FullName))
            );
        services.AddLogging();
        services.AddAuth(configuration);
        return services;
    }
    private static string BuildConnectionString(this IConfiguration configuration, string name)
    {
        var username = configuration.GetValue<string>("DB_USERNAME") ?? throw new InvalidOperationException("Database username not found.");
        var password = configuration.GetValue<string>("DB_PASSWORD") ?? throw new InvalidOperationException("Database password not found.");
        var connectionString = configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
        var sb = new StringBuilder(connectionString);
        sb.Append($"Username={username};Password={password}");
        return sb.ToString();
    }
    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(services.GetRequiredService<IConfiguration>())
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());
        return services;
    }
    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
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
        services.AddAuthorizationBuilder()
            .AddPolicy("JwtPolicy", policy =>
            {
                policy.RequireClaim("iss", jwtOptions.Issuer);
                policy.RequireClaim("aud", jwtOptions.Audience);
                policy.RequireAuthenticatedUser();
            });
        return services;
    }

}
