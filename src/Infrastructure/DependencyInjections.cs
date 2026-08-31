using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Confluent.Kafka;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        services
            .AddLogging()
            .AddRepositories()
            .AddProducer(configuration);
        return services;
    }
    public static string BuildConnectionString(this IConfiguration configuration, string name)
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
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IPhysicianRepository, PhysicianRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();


        return services;
    }
    private static IServiceCollection AddProducer(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddOptionsWithValidateOnStart<KafkaProducer.Options>()
                .Bind(configuration.GetSection(KafkaProducer.Options.SectionName))
                .ValidateDataAnnotations();

        services.AddSingleton((sp) => new ProducerConfig
        {
            BootstrapServers = sp.GetRequiredService<IOptions<KafkaProducer.Options>>().Value.BootstrapServers
        });

        services.AddSingleton<IPublisherEvent, KafkaProducer>();
        return services;
    }
    public static IServiceCollection AddNotifyRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<PushwooshNotificationRepository.Options>()
                .Bind(configuration.GetSection(PushwooshNotificationRepository.Options.SectionName))
                .ValidateDataAnnotations();

        services.AddHttpClient<INotificationRepository, PushwooshNotificationRepository>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PushwooshNotificationRepository.Options>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", options.ApiToken);
        });
        return services;
    }

}
