using Application.Integration.Pushwoosh;
using Application.Interfaces.Services;
using Application.Options;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Application.Factorys;

public sealed class AppointmentNotificationFactory : IAppointmentNotificationFactory
{
    private readonly AppointmentCreatedNotificationOptions _options;

    public AppointmentNotificationFactory(IOptions<AppointmentCreatedNotificationOptions> options)
    {
        _options = options.Value;
    }

    public PushwooshNotifyRequest Create(AppointmentCreatedEvent message, IEnumerable<Device> devices)
    {
        var deviceList = devices?.ToList() ?? [];

        var localized = BuildLocalizedContent(message);

        const string scheduleAtFormat = "yyyy-MM-ddTHH:mm:ssZ";

        var transactional = new NotifyTransactional
        {
            Application = _options.ApplicationCode,
            Schedule = new Integration.Pushwoosh.Schedule
            {
                At = DateTimeOffset.Now.AddDays(-1).ToString(scheduleAtFormat),
                FollowUserTimezone = false,
            },
            Payload = new Payload
            {
                Content = new Content
                {
                    LocalizedContent = localized
                }
            },
            PushTokens = new IdentifierList
            {
                List = deviceList.Select(d => d.PushToken)
            },
            ReturnUnknownIdentifiers = true,
            MessageType = MessageType.Transactional
        };

        return new PushwooshNotifyRequest { Transactional = transactional };
    }

    private Dictionary<string, LocalizedContentItem> BuildLocalizedContent(AppointmentCreatedEvent message)
    {
        var content = new Dictionary<string, LocalizedContentItem>(StringComparer.OrdinalIgnoreCase);

        if (_options.Messages is null || _options.Messages.Count == 0)
        {
            return content;
        }

        foreach (var item in _options.Messages)
        {
            var title = FormatTemplate(item.Value.Title, message, item.Key);
            var body = FormatTemplate(item.Value.Body, message, item.Key);

            var platform = new PlatformContent { Title = title, Body = body };

            content[item.Key] = new LocalizedContentItem
            {
                Android = platform,
                Ios = new PlatformContent { Title = title, Body = body }
            };
        }

        return content;
    }

    private static string FormatTemplate(string template, AppointmentCreatedEvent message, string localeKey)
    {
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        const string placeholderUserName = "{UserName}";
        const string placeholderAppointmentDate = "{AppointmentDate}";
        const string placeholderAppointmentTime = "{AppointmentTime}";

        var result = template.Replace(placeholderUserName, message.User.Name ?? string.Empty);

        var (date, time) = FormatAppointmentDateTime(message.Schedule.StartTime, localeKey);
        result = result.Replace(placeholderAppointmentDate, date).Replace(placeholderAppointmentTime, time);


        return result;
    }

    private static (string Date, string Time) FormatAppointmentDateTime(DateTimeOffset dt, string localeKey)
    {
        const string ptDateFormat = "dd/MM/yyyy";
        const string ptTime24hFormat = "HH:mm";

        const string enDateFormat = "MM/dd/yyyy";
        const string enTime12hFormat = "hh:mm tt";

        const string defaultDateFormat = "yyyy-MM-dd";
        const string defaultTime24hFormat = "HH:mm";

        if (!string.IsNullOrEmpty(localeKey) && localeKey.StartsWith("pt", StringComparison.OrdinalIgnoreCase))
        {
            return (dt.ToString(ptDateFormat, CultureInfo.InvariantCulture), dt.ToString(ptTime24hFormat, CultureInfo.InvariantCulture));
        }

        if (!string.IsNullOrEmpty(localeKey) && localeKey.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        {
            return (dt.ToString(enDateFormat, CultureInfo.InvariantCulture), dt.ToString(enTime12hFormat, CultureInfo.InvariantCulture));
        }

        return (dt.ToString(defaultDateFormat, CultureInfo.InvariantCulture), dt.ToString(defaultTime24hFormat, CultureInfo.InvariantCulture));
    }
}
