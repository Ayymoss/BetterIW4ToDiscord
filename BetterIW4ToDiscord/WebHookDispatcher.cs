using System.Text;
using System.Text.Json;
using BetterIW4ToDiscord.WebHook;
using Microsoft.Extensions.Logging;

namespace BetterIW4ToDiscord;

public class WebHookDispatcher
{
    private readonly ILogger<WebHookDispatcher> _logger;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public WebHookDispatcher(ILogger<WebHookDispatcher> logger)
    {
        _logger = logger;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "BetterIW4ToDiscord");
    }

    public async Task SendWebHookAsync<TWebHook>(string webhook, TWebHook webHookRaw, CancellationToken cancellationToken = default)
        where TWebHook : WebHookRoot
    {
        var serialised = JsonSerializer.Serialize(new { Embeds = new[] { webHookRaw } }, _jsonOptions);
        try
        {
            var response = await _client.PostAsync(webhook, new StringContent(serialised, Encoding.UTF8, "application/json"),
                cancellationToken);
            if (response.IsSuccessStatusCode) return;
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send webhook: {StatusCode} - {Result}", response.StatusCode, result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending webhook");
        }
    }
}
