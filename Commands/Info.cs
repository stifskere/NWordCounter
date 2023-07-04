using Discord;
using Discord.Interactions;
using JetBrains.Annotations;

namespace NwordCounter.Commands;

public class Info : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("info", "What's all this about?"), UsedImplicitly]
    public async Task InfoCommand()
    {
        await DeferAsync();

        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle("About this bot")
            .WithThumbnailUrl(Bot.Client.CurrentUser.GetAvatarUrl())
            .WithDescription(
                "This bot is all about counting how racist you are! Whenever you say the NWord this bot will count it and make a percentile.")
            .AddField("Don't worry", "This data is private unless you want to expose it ||or if you reach the top 5 most racist users||")
            .AddField("Current counted words", string.Join(", ", Bot.CountedWordTypes))
            .AddField("Disclaimer", "The data on this bot is private, and all exposition is done trough the commands you can see.")
            .AddField("Participation", "You can opt out of this bot anytime using the </opt out:1125541780855722066> command and opt in again using the </opt in:1125541780855722066> command.")
            .AddField("Bot source", "This bot is fully open source and made 100% in C#, you can check it [here](https://github.com/stifskere/NWordCounter)")
            .WithColor(Bot.DefaultEmbedColor)
            .WithFooter("glhf -memw");

        await ModifyOriginalResponseAsync(r => r.Embed = embed.Build());
    }
}