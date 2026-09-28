using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Confluent.Kafka;
using Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Jobs.Consumers.Consumers;

public class AppointmentNotificationConsumer
    (
        ILogger<AppointmentNotificationConsumer> logger,
        ConsumerConfig consumerConfig,
        INotificationRepository notificationRepository,
        IDeviceRepository deviceRepository,
        IOptions<AppointmentNotificationConsumer.Options> options,
        IAppointmentNotificationFactory appointmentNotificationFactory
    ) : BackgroundService
{

    private readonly Options optionsValue = options.Value;
    private readonly IAppointmentNotificationFactory _appointmentNotificationFactory = appointmentNotificationFactory;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string topic = "appointment-created";
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            var message = DeserializeEvent(consumeResult.Message.Value);

            if (message?.User is null)
            {
                logger.LogWarning("Received null or invalid message from topic {Topic}", topic);
                continue;
            }

            try
            {
                await ProcessEventAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing appointment-created event {AppointmentId}", message.Id);
            }

            _ = consumer.Commit();
        }
    }

    private AppointmentCreatedEvent? DeserializeEvent(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AppointmentCreatedEvent>(payload);
        }
        catch (JsonException je)
        {
            logger.LogWarning(je, "Failed to deserialize AppointmentCreatedEvent");
            return null;
        }
    }

    private async Task ProcessEventAsync(AppointmentCreatedEvent message, CancellationToken cancellationToken)
    {
        var devices = await deviceRepository.GetDevicesByUserIdAsync(message.User.UserId, cancellationToken);

        if (devices.Count == 0)
        {
            logger.LogWarning("No devices found for user {UserId}", message.User.UserId);
            return;
        }

        var request = _appointmentNotificationFactory.Create(message, devices);

        await notificationRepository.NotifyAsync(request, cancellationToken);
    }

}
