using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using JetBrains.Annotations;
using NwordCounter.Stuff;

namespace NwordCounter.Commands;

[Group("top", "Top users group.")]
public class Top : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("five", "The top 5 most racist users in this guild."), UsedImplicitly]
    public async Task Top5Command()
    {
        await DeferAsync();
    
        EmbedBuilder topEmbed = new EmbedBuilder()
            .WithTitle($":trophy: Top 5 racist users in {Context.Guild.Name} :trophy:")
            .WithColor(0xFFFF00);

        (string, object) gt = ("guild", Context.Guild.Id);
        DatabaseResult topFive = Bot.Database.Exec("SELECT user, count, normalCount FROM UserNwords WHERE guild = @guild AND count != 0 AND normalCount != 0 ORDER BY count DESC LIMIT 5", gt);

        if (!topFive.HasResult)
        {
            topEmbed = topEmbed
                .WithDescription("No one said the NWord yet, is this server religious or sum?")
                .WithColor(0xFF0000);

            await ModifyOriginalResponseAsync(r => r.Embed = topEmbed.Build());
            return;
        }
    
        long totalCount = Bot.Database.Exec("SELECT count FROM GuildNwords WHERE guild = @guild", gt).GetValue<long>("count", 1);

        topEmbed = topEmbed
            .WithFooter($"The NWord was said {totalCount} times in this guild.");

        for (int i = 1; i <= (topFive.RowCount >= 5 ? 5 : topFive.RowCount); i++)
        {
            ulong userId = (ulong)topFive.GetValue<long>("user", i);

            if (OptManager.UserOptedOut(userId))
                continue;
            
            SocketGuildUser? user = Context.Guild.GetUser(userId);
            long count = topFive.GetValue<long>("count", i), normalCount = topFive.GetValue<long>("normalCount", i);
            topEmbed.AddField($"Top {i.AddSuffix()}", $"**Username:** {user?.Username ?? $"(not found) [{userId}]"}\n**Count:** {count}\n**Percentile:** {Math.Round(100 * (count / (double)normalCount), 2)}%", true);
        }

        await ModifyOriginalResponseAsync(r => r.Embed = topEmbed.Build());
    }

    [SlashCommand("self", "How many times you said the NWord in this guild."), UsedImplicitly]
    public async Task TopSelfCommand([Summary("expose", "Expose your count to other users.")]bool expose = false)
    {
        await DeferAsync(!expose);

        if (OptManager.UserOptedOut(Context.User.Id))
        {
            EmbedBuilder optOutEmbed = new EmbedBuilder()
                .WithTitle("You opted out")
                .WithDescription("You can't see your info till you opt in again.")
                .WithFooter("You make me sad :(")
                .WithColor(0xFF0000);

            await ModifyOriginalResponseAsync(r => r.Embed = optOutEmbed.Build());
            return;
        }

        DatabaseResult userStats = Bot.Database.Exec("SELECT count, normalCount FROM UserNwords WHERE guild = @guild AND user = @user", 
            ("guild", Context.Guild.Id), ("user", Context.User.Id));

        if (!userStats.HasResult)
        {
            EmbedBuilder errorEmbed = new EmbedBuilder()
                .WithTitle("Good boy!")
                .WithDescription("You didn't say the NWord yet, you are a so non cancellable person, yet i don't like you.")
                .WithColor(0xFF0000);

            await ModifyOriginalResponseAsync(r => r.Embed = errorEmbed.Build());
            return;
        }
        
        long nWordUsage = userStats.GetValue<long>("count", 1), normalMessages = userStats.GetValue<long>("normalCount", 1);
        
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle($"NWord usage in {Context.Guild.Name}")
            .WithDescription($"You used the NWord {nWordUsage} times in this guild, and you used the NWord in the {Math.Round(100 * (nWordUsage / (double)normalMessages), 2)}% of your messages in this guild.")
            .WithFooter(expose ? "You decided to expose the message you little imp!" : "This data is hidden for your privacy and safety :)")
            .WithColor(Bot.DefaultEmbedColor);

        await ModifyOriginalResponseAsync(r => r.Embed = embed.Build());
    }
}