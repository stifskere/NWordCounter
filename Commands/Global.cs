using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using NwordCounter.Stuff;

namespace NwordCounter.Commands;

[Group("global", "Global stats group"), UsedImplicitly]
public class Global : InteractionModuleBase<SocketInteractionContext>
{
    [Group("top", "Global top stats group"), UsedImplicitly]
    public class Top : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("five", "Global top 5 most racist users."), UsedImplicitly]
        public async Task Top5Command()
        {
            await DeferAsync();
            
            DatabaseResult topFiveData = Bot.Database.Exec("SELECT user, guild, count, normalCount FROM UserNwords WHERE count != 0 AND normalCount != 0");
            
            Dictionary<ulong, (long count, long normalCount)> counts = new();

            foreach (object[] objects in topFiveData)
            {
                if (counts.ContainsKey((ulong)(long)objects[0]))
                {
                    (long count, long normalCount) valueTuple = counts[(ulong)(long)objects[0]];
                    valueTuple.count += (long)objects[2];
                    valueTuple.normalCount += (long)objects[3];
                    counts[(ulong)(long)objects[0]] = valueTuple;
                    continue;
                }
                
                counts.Add((ulong)(long)objects[0], ((long)objects[2], (long)objects[3]));
            }

            List<KeyValuePair<ulong, (long count, long normalCount)>> orderedCounts = 
                counts.OrderByDescending(v => v.Value.count)
                    .Take(5).ToList();

            EmbedBuilder top5Embed = new EmbedBuilder()
                .WithTitle(":trophy: Global top 5 most racist users :trophy:")
                .WithColor(0xFFFF00);

            if (orderedCounts.Count == 0)
            {
                top5Embed = top5Embed
                    .WithColor(0xFF0000)
                    .WithDescription("Seems like nobody was registered being racist, they are such angels. (i hate them.)");
            }
            
            for (int i = 0; i < (orderedCounts.Count >= 5 ? 5 : orderedCounts.Count); i++)
            {
                IUser topUserData = await Bot.Client.GetUserAsync(orderedCounts[i].Key);
                long normalCount = orderedCounts[i].Value.normalCount, count = orderedCounts[i].Value.count;
                top5Embed.AddField($"Top {(i + 1).AddSuffix()}", $"**Username:** {topUserData.Username}\n**Count:** {count}\n**Percentile:** {Math.Round(100 * (count / (double)normalCount), 2)}%", true);
            }

            await ModifyOriginalResponseAsync(r => r.Embed = top5Embed.Build());
        }

        [SlashCommand("self", "How many times you said the NWord globally."), UsedImplicitly]
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
            
            (long count, long normalCount) userCounts = (0, 0);
            foreach (object[] row in Bot.Database.Exec("SELECT user, guild, count, normalCount FROM UserNwords WHERE user = @user", ("user", Context.User.Id)))
            {
                userCounts.normalCount += (long)row[3];
                userCounts.count += (long)row[2];
            }

            if (userCounts.count == 0 || userCounts.normalCount == 0)
            {
                EmbedBuilder errorEmbed = new EmbedBuilder()
                    .WithTitle("Unbelievable!")
                    .WithDescription("I can't believe that, you didn't say the nword anywhere anytime? damn.")
                    .WithColor(0xFF0000);

                await ModifyOriginalResponseAsync(r => r.Embed = errorEmbed.Build());
                return;
            }
            
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("Global NWord usage")
                .WithDescription($"You used the NWord {userCounts.count} times, basically the {Math.Round(100 * (userCounts.count / (double)userCounts.normalCount), 2)}% of your global messages.")
                .WithFooter(expose ? "You decided to expose the message you little imp!" : "This data is hidden for your privacy and safety :)")
                .WithColor(Bot.DefaultEmbedColor);

            await ModifyOriginalResponseAsync(r => r.Embed = embed.Build());
        }
    }
}