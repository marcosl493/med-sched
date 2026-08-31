using Application.Integration.Pushwoosh;
using Application.Interfaces.Repositories;
using Confluent.Kafka;
using Domain.Events;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace Jobs.Consumers.Consumers;

public class AppointmentNotificationConsumer
    (
        ILogger<AppointmentNotificationConsumer> logger,
        ConsumerConfig consumerConfig,
        INotificationRepository notificationRepository,
        IDeviceRepository deviceRepository,
        IOptions<AppointmentNotificationConsumer.Options> options
    ) : BackgroundService
{
    public sealed class Options
    {
        public const string SectionName = "AppointmentCreatedNotificationConsumer";
        public Dictionary<string, Message> Messages { get; set; } = [];
        public sealed class Message
        {
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }
        public string ApplicationCode { get; set; } = string.Empty;
    }
    private readonly Options optionsValue = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string topic = "appointment-created";
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            var message = JsonSerializer.Deserialize<AppointmentCreatedEvent>(consumeResult.Message.Value);
            if (message?.User is null)
            {
                logger.LogWarning("Received null message from topic {Topic}", topic);
                continue;
            }
            var devices = await deviceRepository.GetDevicesByUserIdAsync(message.User.UserId, stoppingToken);

            if (devices.Count == 0)
            {
                logger.LogWarning("No devices found for user {UserId}", message.User.UserId);
                continue;
            }

            var content = new Dictionary<string, LocalizedContentItem>();

            foreach (var item in optionsValue.Messages)
            {
                content.TryAdd(item.Key, new LocalizedContentItem
                {
                    Android = new PlatformContent
                    {
                        Title = FormatTemplate(item.Value.Title, message, item.Key),
                        Body = FormatTemplate(item.Value.Body, message, item.Key)
                    },
                    Ios = new PlatformContent
                    {
                        Title = FormatTemplate(item.Value.Title, message, item.Key),
                        Body = FormatTemplate(item.Value.Body, message, item.Key)
                    }
                });
            }

            var notificationRequest = new PushwooshNotifyRequest
            {
                Transactional = new NotifyTransactional
                {
                    Application = optionsValue.ApplicationCode,
                    Schedule = new Schedule
                    {
                        At = DateTimeOffset.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        FollowUserTimezone = false,
                    },
                    Payload = new Payload
                    {
                        Content = new Content
                        {
                            LocalizedContent = content
                        }
                    },
                    PushTokens = new IdentifierList
                    {
                        List = devices.Select(d => d.PushToken)
                    },
                    ReturnUnknownIdentifiers = true,
                    MessageType = "MESSAGE_TYPE_TRANSACTIONAL"
                }
            };
            var response = await notificationRepository.NotifyAsync(notificationRequest, stoppingToken);

            _ = consumer.Commit();
        }
    }

    private static string FormatTemplate(string template, AppointmentCreatedEvent message, string localeKey)
    {
        if (string.IsNullOrEmpty(template))
        {
            return template ?? string.Empty;
        }
        var result = template;

        if (message is { Schedule: not null } && !string.IsNullOrEmpty(localeKey))
        {
            result = result.Replace("{UserName}", message.User.Name ?? string.Empty);

            if (localeKey.StartsWith("pt", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Replace("{AppointmentDate}", message.Schedule.StartTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                result = result.Replace("{AppointmentTime}", message.Schedule.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture));
            }
            else if (!string.IsNullOrEmpty(localeKey) && localeKey.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Replace("{AppointmentDate}", message.Schedule.StartTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                result = result.Replace("{AppointmentTime}", message.Schedule.StartTime.ToString("hh:mm tt", CultureInfo.InvariantCulture));
            }
            else
            {
                result = result.Replace("{AppointmentDate}", message.Schedule.StartTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                result = result.Replace("{AppointmentTime}", message.Schedule.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture));
            }

        }
        return result;
    }
}
