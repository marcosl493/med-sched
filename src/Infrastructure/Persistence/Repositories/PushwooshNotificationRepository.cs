using Application.Integration.Pushwoosh;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Persistence.Repositories;

public class PushwooshNotificationRepository : INotificationRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PushwooshNotificationRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    public PushwooshNotificationRepository(HttpClient httpClient, ILogger<PushwooshNotificationRepository> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PushwooshNotifyResponse?> NotifyAsync(PushwooshNotifyRequest request, CancellationToken cancellationToken = default)
    {
        const string resource = "messaging/v2/notify";
        using var response = await _httpClient.PostAsJsonAsync(resource, request, _jsonOptions, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Erro ao se comunicar com a API do Pushwoosh. {Content}", await response.Content.ReadAsStringAsync(cancellationToken));
        }

        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<PushwooshApiResponse>(cancellationToken: cancellationToken);
        return apiResponse?.Result;
    }
    public sealed class Options
    {
        public const string SectionName = "Pushwoosh";
        [Required]
        public string BaseUrl { get; set; } = string.Empty;
        [Required]
        public string ApiToken { get; set; } = string.Empty;

    }
}
