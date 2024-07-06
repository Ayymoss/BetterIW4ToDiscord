using BetterIW4ToDiscord.Configuration;
using BetterIW4ToDiscord.Utilities;
using BetterIW4ToDiscord.WebHook.Components;
using BetterIW4ToDiscord.WebHook.Hooks;
using Data.Models;
using Humanizer;
using SharedLibraryCore;
using SharedLibraryCore.Configuration;
using SharedLibraryCore.Database.Models;
using SharedLibraryCore.Interfaces;

namespace BetterIW4ToDiscord;

public class WebHookManager(ConfigurationRoot config, WebHookDispatcher dispatcher, ApplicationConfiguration appConfig)
{
    public async Task OnClientPenaltyAsync(EFClient client, EFPenalty penalty, CancellationToken token = default)
    {
        var server = client.CurrentServer;
        var parser = Resources.GetParser(server.RconParser.Name);
        var offence = penalty.Offense.StripColors();
        var duration = penalty.Expires is null ? "Permanent" : penalty.Expires.Humanize();
        var target = client.ClientToUrl(appConfig.WebfrontUrl);
        var issuer = penalty.Punisher.ToPartialClient().ClientToUrl(appConfig.WebfrontUrl);

        switch (penalty.Type)
        {
            case EFPenalty.PenaltyType.Report:
                await BuildClientReportEmbedAsync(server, parser, issuer, target, offence, token);
                break;
            case EFPenalty.PenaltyType.Kick:
                List<Field> kickFields =
                [
                    new Field
                    {
                        Name = "Reason",
                        Value = offence,
                        Inline = false
                    },
                    new Field
                    {
                        Name = "Server",
                        Value = server.ServerName.StripColors(),
                        Inline = true
                    }
                ];
                await BuildClientPenaltyEmbedAsync(parser, $"{issuer} kicked {target}", 15466496,
                    kickFields, token);
                break;
            case EFPenalty.PenaltyType.TempBan:
                List<Field> tempBanFields =
                [
                    new Field
                    {
                        Name = "Reason",
                        Value = offence,
                        Inline = false
                    },
                    new Field
                    {
                        Name = "Server",
                        Value = server.ServerName.StripColors(),
                        Inline = true
                    },
                    new Field
                    {
                        Name = "Duration",
                        Value = duration,
                        Inline = true
                    }
                ];

                await BuildClientPenaltyEmbedAsync(parser, $"{issuer} temp banned {target}", 15466496,
                    tempBanFields, token);
                break;
            case EFPenalty.PenaltyType.Ban:
                List<Field> banFields =
                [
                    new Field
                    {
                        Name = "Reason",
                        Value = offence,
                        Inline = false
                    },
                    new Field
                    {
                        Name = "Server",
                        Value = server.ServerName.StripColors(),
                        Inline = true
                    },
                    new Field
                    {
                        Name = "Duration",
                        Value = "Permanent",
                        Inline = true
                    }
                ];

                await BuildClientPenaltyEmbedAsync(parser, $"{issuer} banned {target}", 15466496,
                    banFields, token);
                break;
            case EFPenalty.PenaltyType.Unban:
                List<Field> unbanFields =
                [
                    new Field
                    {
                        Name = "Reason",
                        Value = offence,
                        Inline = false
                    }
                ];

                await BuildClientPenaltyEmbedAsync(parser, $"{issuer} unbanned {target}", 15132390,
                    unbanFields, token);
                break;
        }
    }

    private async Task BuildClientPenaltyEmbedAsync(Resources.Parser map, string description, int colour, List<Field> fields,
        CancellationToken token = default)
    {
        var message = new ClientPenaltyHook
        {
            Description = description,
            DateTimeOffset = DateTimeOffset.Now,
            Colour = colour,
            Author = new Author
            {
                Name = map.Name,
                IconUri = map.IconUri,
            },
            Fields = fields
        };

        await dispatcher.SendWebHookAsync(config.WebHooks.ClientPenalty, message, token);
    }

    private async Task BuildClientReportEmbedAsync(IGameServer server, Resources.Parser parser, string issuerUrl, string targetUrl,
        string offence, CancellationToken token = default)
    {
        var onlineAdmins = server.ConnectedClients
            .Where(x => x.Level is not Data.Models.Client.EFClient.Permission.User)
            .Where(x => x.Level is not Data.Models.Client.EFClient.Permission.Trusted)
            .Where(x => x.Level is not Data.Models.Client.EFClient.Permission.Flagged)
            .Where(x => !x.Masked).Select(x => x.CleanedName).ToList();

        var message = new ClientReportHook
        {
            Description = $"{issuerUrl} reported {targetUrl}",
            DateTimeOffset = DateTimeOffset.Now,
            Colour = parser.Colour,
            Author = new Author
            {
                Name = parser.Name,
                IconUri = parser.IconUri,
            },
            Footer = new Footer
            {
                Text = $"Online Admins: {(onlineAdmins.Count is 0 ? "No admins online" : string.Join(", ", onlineAdmins))}"
            },
            Thumbnail = new Thumbnail
            {
                Uri = Resources.GetMap(server).ImageUri
            },
            Fields =
            [
                new Field
                {
                    Name = "Server",
                    Value = server.ServerName.StripColors(),
                    Inline = false
                },
                new Field
                {
                    Name = "Reason",
                    Value = offence,
                    Inline = false
                }
            ]
        };

        await dispatcher.SendWebHookAsync(config.WebHooks.ClientReport, message, token);
    }

    public async Task BuildServerStatusEmbedAsync(IGameServer server, string description, int colour, CancellationToken token = default)
    {
        var parser = Resources.GetParser(server.RconParser.Name);

        var message = new ServerStatusHook
        {
            Description = $"{description} **{server.ServerName.StripColors()}**",
            DateTimeOffset = DateTimeOffset.Now,
            Colour = colour,
            Author = new Author
            {
                Name = parser.Name,
                IconUri = parser.IconUri,
            },
            Title = "Server Status"
        };

        await dispatcher.SendWebHookAsync(config.WebHooks.ServerStatus, message, token);
    }

    public async Task BuildClientConnectionEmbedAsync(EFClient client, string description, int colour, CancellationToken token = default)
    {
        var server = client.CurrentServer;
        var parser = Resources.GetParser(server.RconParser.Name);

        var message = new ClientConnectionHook
        {
            Description = description,
            DateTimeOffset = DateTimeOffset.Now,
            Colour = colour,
            Author = new Author
            {
                Name = parser.Name,
                IconUri = parser.IconUri,
            },
            Footer = new Footer
            {
                Text = server.ServerName.StripColors()
            },
            Title = client.CleanedName
        };

        await dispatcher.SendWebHookAsync(config.WebHooks.ClientConnection, message, token);
    }
}
