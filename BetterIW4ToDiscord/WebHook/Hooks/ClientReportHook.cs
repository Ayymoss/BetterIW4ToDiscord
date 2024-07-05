using BetterIW4ToDiscord.WebHook.Components;

namespace BetterIW4ToDiscord.WebHook.Hooks;

public class ClientReportHook : WebHookRoot
{
    public required Footer Footer { get; set; }
    public required Thumbnail Thumbnail { get; set; }
    public required List<Field> Fields { get; set; }

}
