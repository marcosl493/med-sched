namespace Application.Options;

public sealed class AppointmentCreatedNotificationOptions
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
