using System.Text.Json;
using BetterIW4ToDiscord.Configuration;
using BetterIW4ToDiscord.WebHook.Components;
using BetterIW4ToDiscord.WebHook.Hooks;
using Microsoft.Extensions.DependencyInjection;
using SharedLibraryCore;
using SharedLibraryCore.Configuration;
using SharedLibraryCore.Events.Game;
using SharedLibraryCore.Events.Management;
using SharedLibraryCore.Events.Server;
using SharedLibraryCore.Interfaces;
using SharedLibraryCore.Interfaces.Events;

namespace BetterIW4ToDiscord;

public class Plugin : IPluginV2
{
    private readonly WebHookDispatcher _webHookDispatcher;
    private readonly WebHookManager _webhookManager;
    private readonly ConfigurationRoot _config;
    private readonly Resources _resources;
    public string Name => "Better IW4 To Discord";
    public string Author => "Amos";
    public string Version => "2025-11-16";

    public Plugin(WebHookDispatcher webHookDispatcher, WebHookManager webhookManager, ConfigurationRoot config, Resources resources)
    {
        _webHookDispatcher = webHookDispatcher;
        _webhookManager = webhookManager;
        _config = config;
        _resources = resources;

        foreach (var property in config.WebHooks.GetType().GetProperties())
        {
            if (property.PropertyType != typeof(string)) continue;
            var value = property.GetValue(config.WebHooks) as string;
            if (!string.IsNullOrEmpty(value)) continue;
            Console.WriteLine($"[{Name}] Unloaded. Configuration property {property.Name} is empty.");
            return;
        }

        IManagementEventSubscriptions.ClientPenaltyAdministered += OnClientPenaltyAdministered;
        IManagementEventSubscriptions.ClientPenaltyRevoked += OnClientPenaltyRevoked;
        IManagementEventSubscriptions.ClientStateAuthorized += OnClientStateAuthorized;
        IManagementEventSubscriptions.ClientStateDisposed += OnClientStateDisposed;

        IGameServerEventSubscriptions.ConnectionInterrupted += OnConnectionInterrupted;
        IGameServerEventSubscriptions.ConnectionRestored += OnConnectionRestored;
        IGameServerEventSubscriptions.MonitoringStarted += OnMonitoringStarted;

        IGameEventSubscriptions.ClientMessaged += OnClientMessaged;

        IManagementEventSubscriptions.Load += OnLoad;
    }

    public static void RegisterDependencies(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<WebHookDispatcher>();
        serviceCollection.AddSingleton<WebHookManager>();
        serviceCollection.AddSingleton<Resources>();
        serviceCollection.AddConfiguration("BetterIW4ToDiscord", new ConfigurationRoot());
    }

    private Task OnLoad(IManager manager, CancellationToken token)
    {
        Console.WriteLine($"[{Name}] loaded. Version: {Version}");
        return Task.CompletedTask;
    }

    private async Task OnClientMessaged(ClientMessageEvent messageEvent, CancellationToken token)
    {
        var map = _resources.GetParser(messageEvent.Server.RconParser.Name);

        var message = new ClientMessageHook
        {
            Title = messageEvent.Client.CleanedName,
            Footer = new Footer
            {
                Text = messageEvent.Server.ServerName.StripColors()
            },
            Description = messageEvent.Message.StripColors(),
            DateTimeOffset = DateTimeOffset.Now,
            Colour = 3564200,
            Author = new Author
            {
                Name = map.Name,
                IconUri = map.IconUri
            }
        };

        await _webHookDispatcher.SendWebHookAsync(_config.WebHooks.ClientMessage, message, token);
    }

    private async Task OnMonitoringStarted(MonitorStartEvent startEvent, CancellationToken token) =>
        await _webhookManager.BuildServerStatusEmbedAsync(startEvent.Server, "Monitoring started on", 3394699, token);

    private async Task OnConnectionRestored(ConnectionRestoreEvent restoreEvent, CancellationToken token) =>
        await _webhookManager.BuildServerStatusEmbedAsync(restoreEvent.Server, "Restored started to", 3394611, token);

    private async Task OnConnectionInterrupted(ConnectionInterruptEvent interruptEvent, CancellationToken token) =>
        await _webhookManager.BuildServerStatusEmbedAsync(interruptEvent.Server, "Lost connection to", 15073280, token);

    private async Task OnClientStateDisposed(ClientStateDisposeEvent disposeEvent, CancellationToken token) =>
        await _webhookManager.BuildClientConnectionEmbedAsync(disposeEvent.Client, "Disconnected", 10029348, token);

    private async Task OnClientStateAuthorized(ClientStateAuthorizeEvent authorizeEvent, CancellationToken token) =>
        await _webhookManager.BuildClientConnectionEmbedAsync(authorizeEvent.Client, "Connected", 96820, token);

    private async Task OnClientPenaltyRevoked(ClientPenaltyRevokeEvent revokeEvent, CancellationToken token) =>
        await _webhookManager.OnClientPenaltyAsync(revokeEvent.Client, revokeEvent.Penalty, token);

    private async Task OnClientPenaltyAdministered(ClientPenaltyEvent penaltyEvent, CancellationToken token) =>
        await _webhookManager.OnClientPenaltyAsync(penaltyEvent.Client, penaltyEvent.Penalty, token);
}
