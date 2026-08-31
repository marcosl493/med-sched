using System.Text.Json.Serialization;

namespace Application.Integration.Pushwoosh;

public class PushwooshNotifyRequest
{
    [JsonPropertyName("segment")]
    public NotifySegment? Segment { get; init; }

    [JsonPropertyName("transactional")]
    public NotifyTransactional? Transactional { get; init; }
}
public static class MessageType
{
    public const string Transactional = "MESSAGE_TYPE_TRANSACTIONAL";
    public const string Marketing = "MESSAGE_TYPE_MARKETING";
}
public class NotifySegment
{
    public string? Application { get; init; }
    public string[]? Platforms { get; init; }
    public string? Code { get; init; }
    public Payload? Payload { get; init; }
    public Schedule? Schedule { get; init; }
    public string? MessageType { get; init; }
}

public class NotifyTransactional
{
    public string? Application { get; init; }
    public string[]? Platforms { get; init; }
    public IdentifierList? Hwids { get; init; }
    public IdentifierList? Users { get; init; }
    public IdentifierList? PushTokens { get; init; }
    public Payload Payload { get; init; } = new();
    public Schedule Schedule { get; init; } = new();
    public bool? ReturnUnknownIdentifiers { get; init; }
    public bool? UseLatestUserDevice { get; init; }
    public string? MessageType { get; init; }
}

public class IdentifierList { public IEnumerable<string>? List { get; init; } }

public class Schedule
{
    public string At { get; init; } = string.Empty;
    public string? After { get; init; }
    public bool FollowUserTimezone { get; init; }
    public string? PastTimezonesBehaviour { get; init; }
}

public class Payload
{
    public Content Content { get; init; } = new();
}

public class Content
{
    public Dictionary<string, LocalizedContentItem> LocalizedContent { get; init; } = [];
}

public class LocalizedContentItem
{
    [JsonPropertyName("ios")]
    public PlatformContent Ios { get; init; } = new();

    [JsonPropertyName("android")]
    public PlatformContent Android { get; init; } = new();
}

public class PlatformContent
{
    public string? Title { get; init; }
    public string? Body { get; init; }
}

public record PushwooshNotifyResponse(
    [property: JsonPropertyName("message_code")] string? MessageCode,
    [property: JsonPropertyName("unknown_identifiers")] string[]? UnknownIdentifiers);

public class PushwooshApiResponse
{
    public PushwooshNotifyResponse? Result { get; init; }
}
