using System.Text.Json.Serialization;

namespace BetterIW4ToDiscord.WebHook.Components;

public class Author
{
    public required string Name { get; set; }
    [JsonPropertyName("icon_url")]public required string IconUri { get; set; }
}
