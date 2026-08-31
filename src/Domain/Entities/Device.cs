namespace Domain.Entities;

public class Device
{
    public Guid Hwid { get; set; }
    public virtual User User { get; set; } = null!;
    public Guid UserId { get; set; }
    public string PushToken { get; private set; } = string.Empty;
    public string ApplicationVersion { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string DeviceModel { get; private set; } = string.Empty;
    public string Language { get; private set; } = string.Empty;
    public string Platform { get; private set; } = string.Empty;
    public bool PushAlertsEnabled { get; private set; }
    public string SdkVersion { get; private set; } = string.Empty;
    public bool ScheduledSummary { get; private set; }
    public bool TimeSensitiveNotifications { get; private set; }
    public int Timezone { get; private set; }
    public string ApplicationCode { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; } = DateTime.UtcNow;
    public Device()
    {

    }

    public Device
        (
            Guid hwid,
            string pushToken,
            string applicationVersion,
            string city,
            string country,
            string deviceModel,
            string language,
            string platform,
            bool pushAlertsEnabled,
            string sdkVersion,
            bool scheduledSummary,
            bool timeSensitiveNotifications,
            int timezone,
            Guid userId)
    {
        Hwid = hwid;
        PushToken = pushToken;
        ApplicationVersion = applicationVersion;
        City = city;
        Country = country;
        DeviceModel = deviceModel;
        Language = language;
        Platform = platform;
        PushAlertsEnabled = pushAlertsEnabled;
        SdkVersion = sdkVersion;
        ScheduledSummary = scheduledSummary;
        TimeSensitiveNotifications = timeSensitiveNotifications;
        Timezone = timezone;
        UserId = userId;
    }
}
