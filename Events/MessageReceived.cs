
using Discord;
using Discord.WebSocket;
using JetBrains.Annotations;
using NwordCounter.Stuff;

namespace NwordCounter.Events;

public static partial class Events
{
    [Event("MessageReceived"), UsedImplicitly]
    public static Task MessageReceivedEvent(SocketMessage message)
    {
        if (OptManager.UserOptedOut(message.Author.Id))
            return Task.CompletedTask;
        
        string[] splitMessage = message.Content.ToLower().Split(" ");
        
        IGuild guild = ((IGuildChannel)message.Channel).Guild;
        (string, object) gt = ("guild", guild.Id), at = ("author", message.Author.Id);
        
        Bot.Database.Exec("INSERT OR IGNORE INTO UserNwords(user, guild, count, normalCount) VALUES(@author, @guild, 0, 0);", gt, at);
        Bot.Database.Exec("INSERT OR IGNORE INTO GuildNwords(guild, count) VALUES(@guild, 0)", gt);

        Bot.Database.Exec("UPDATE UserNwords SET normalCount = normalCount + 1 WHERE user = @author AND guild = @guild", gt, at);
        
        if (!splitMessage.Any(x => Bot.CountedWordTypes.Contains(x))) 
            return Task.CompletedTask;
        
        Bot.Database.Exec("UPDATE UserNwords SET count = count + 1 WHERE guild = @guild AND user = @author", gt, at);
        Bot.Database.Exec("UPDATE GuildNwords SET count = count + 1 WHERE guild = @guild", gt);

        return Task.CompletedTask;
    }
}