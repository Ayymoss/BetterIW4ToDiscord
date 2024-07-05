using System.Text.Json.Serialization;

namespace BetterIW4ToDiscord.WebHook.Components;

public class Thumbnail
{
    [JsonPropertyName("url")] public required string Uri { get; set; }
}
