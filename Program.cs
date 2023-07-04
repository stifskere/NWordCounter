
using System.Reflection;
using Discord;
using Discord.WebSocket;
using NwordCounter.Stuff;

namespace NwordCounter;

public static class Bot
{
    public static readonly DiscordSocketClient Client = new(new DiscordSocketConfig{GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.Guilds, AlwaysDownloadUsers = true});
    private static readonly FileVariables Secrets = new("./.secrets");
    public static readonly DatabaseHandler Database = new("./data.db");
    
    public static readonly string[] CountedWordTypes = {"niggers", "nigers", "niga", "nigga", "niggas", "niger", "nigas", "nigger"};
    public const uint DefaultEmbedColor = 0x65977d;
    
    public static async Task Main()
    {
        if (!Secrets.EntryExists("TOKEN"))
            Environment.Exit(-1);

        Event.RegisterEvents(Client, Assembly.GetExecutingAssembly());

        await Client.LoginAsync(TokenType.Bot, Secrets["TOKEN"]);
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}