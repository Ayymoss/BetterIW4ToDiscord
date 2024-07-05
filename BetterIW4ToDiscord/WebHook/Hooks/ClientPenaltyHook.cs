using BetterIW4ToDiscord.WebHook.Components;

namespace BetterIW4ToDiscord.WebHook.Hooks;

public class ClientPenaltyHook : WebHookRoot
{
    public required List<Field> Fields { get; set; }
}
