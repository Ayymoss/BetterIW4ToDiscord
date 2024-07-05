using System.Text.Json.Serialization;
using BetterIW4ToDiscord.WebHook.Components;

namespace BetterIW4ToDiscord.WebHook;

public class WebHookRoot
{
    public required string Description { get; set; }
    [JsonPropertyName("timestamp")] public required DateTimeOffset DateTimeOffset { get; set; }
    [JsonPropertyName("color")] public required int Colour { get; set; }
    public required Author Author { get; set; }
}
