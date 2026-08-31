using Application.Interfaces.Repositories;
using Confluent.Kafka;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Jobs.Consumers.Consumers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOptionsWithValidateOnStart<AppointmentNotificationConsumer.Options>()
    .BindConfiguration(AppointmentNotificationConsumer.Options.SectionName)
    .ValidateDataAnnotations();

builder
    .Services
    .AddDbContext<MedSchedDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.BuildConnectionString(nameof(MedSchedDbContext))
            ),
            ServiceLifetime.Singleton
    )
    .AddSingleton<IDeviceRepository, DeviceRepository>()
    .AddNotifyRepository(builder.Configuration);

builder.Services.AddSingleton(sp => new ConsumerConfig
{
    BootstrapServers = builder.Configuration.GetValue<string>("Kafka:Consumer:BootstrapServers") ?? throw new InvalidOperationException("Kafka bootstrap servers not found."),
    AutoOffsetReset = AutoOffsetReset.Earliest,
    GroupId = builder.Configuration.GetValue<string>("Kafka:Consumer:GroupId") ?? throw new InvalidOperationException("Kafka consumer group id not found."),
    EnableAutoCommit = false
});

builder.Services.AddHostedService<AppointmentNotificationConsumer>();

var host = builder.Build();
host.Run();
