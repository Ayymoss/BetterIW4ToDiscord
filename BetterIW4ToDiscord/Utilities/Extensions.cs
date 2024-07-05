using SharedLibraryCore;
using SharedLibraryCore.Database.Models;

namespace BetterIW4ToDiscord.Utilities;

public static class Extensions
{
    public static string ClientToUrl(this EFClient client, string webFrontUrl)
    {
        var name = client.CleanedName ?? client.CurrentAlias.Name.StripColors();
        return client.ClientId is 1 ? "IW4MAdmin" : $"[{name}]({webFrontUrl}/Client/Profile/{client.ClientId})";
    }
}
