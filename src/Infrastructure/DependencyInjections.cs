using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text;

namespace Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddDbContext<MedSchedDbContext>((sp, options) =>
            options.UseNpgsql
            (
                sp.GetRequiredService<IConfiguration>().BuildConnectionString(nameof(MedSchedDbContext)),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(MedSchedDbContext).Assembly.FullName))
            );
        services.AddLogging();
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

}
