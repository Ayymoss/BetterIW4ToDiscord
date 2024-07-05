# [IW4MAdmin](https://github.com/RaidMax/IW4M-Admin) [![GitHub license](https://img.shields.io/github/license/RaidMax/IW4M-Admin)](https://github.com/Zwambro/iw4madmin-plugin-iw4todiscord/blob/master/LICENSE) [![GitHub stars](https://img.shields.io/github/stars/RaidMax/IW4M-Admin)](https://github.com/RaidMax/IW4M-Admin/stargazers)

# BetterIW4ToDiscord, a IW4MAdmin plugin

## Installation
1. Subscribe to the plugin on https://store.raidmax.org/plugins
2. Restart IW4MAdmin.
3. Create necessary channels in your Discord server.
4. Create the webhooks for each channel.
5. Open `IW4MAdmin/Configuration/BetterIW4ToDiscord.json`, you'll see something like:
```json
"WebHooks": {
  "ServerStatus": "",
  "ClientReport": "",
  "ClientPenalty": "",
  "ClientMessage": "",
  "ClientConnection": ""
}
```
6. Replace the empty strings with respective webhooks.
7. Restart IW4MAdmin.

## Requirement
- You may need IW4MAdmin version `2024.6.29.807` or later.

### Special thanks and acknowledgements
- [Zwambro](https://github.com/Zwambro) for the original plugin.
