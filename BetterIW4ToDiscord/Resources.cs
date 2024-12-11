using BetterIW4ToDiscord.Configuration;
using SharedLibraryCore.Interfaces;

namespace BetterIW4ToDiscord;

public class Resources(ConfigurationRoot configuration)
{
    public record Parser(string Name, string IconUri, int Colour);

    public record Map(string Name, string ImageUri);

    public Parser GetParser(string parserName) => configuration.ParserMap.TryGetValue(parserName, out var parser)
        ? parser
        : new Parser("Unknown Game", "https://www.freeiconspng.com/uploads/csgo-icon-4.png", 0);

    public Map GetMap(IGameServer server)
    {
        var maps = configuration.GameImageMap.GetValueOrDefault(server.GameCode.ToString());
        return maps?.FirstOrDefault(x => x.Name.Equals(server.Map.Name)) ?? new Map("Unknown Map",
            "https://cdn0.iconfinder.com/data/icons/flat-design-basic-set-1/24/error-exclamation-512.png");
    }
}
