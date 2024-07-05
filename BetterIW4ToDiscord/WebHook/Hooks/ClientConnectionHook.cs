using BetterIW4ToDiscord.WebHook.Components;

namespace BetterIW4ToDiscord.WebHook.Hooks;

public class ClientConnectionHook : WebHookRoot
{
    public required Footer Footer { get; set; }
    public required string Title { get; set; }
}
