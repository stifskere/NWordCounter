using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using JetBrains.Annotations;
using NwordCounter.Stuff;

namespace NwordCounter.Commands;

[Group("opt", "opt group")]
public class Opt : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("in", "Opt in the bot counter."), UsedImplicitly]
    public async Task OptInCommand()
    {
        await DeferAsync(true);

        if (!OptManager.OptInUser(Context.User.Id))
        {
            EmbedBuilder didntOptOut = new EmbedBuilder()
                .WithTitle("You are in")
                .WithDescription("You can't opt in because you are already in.")
                .WithColor(Bot.DefaultEmbedColor);

            await ModifyOriginalResponseAsync(r => r.Embed = didntOptOut.Build());
            return;
        }

        EmbedBuilder optInEmbed = new EmbedBuilder()
            .WithTitle(":cold_face: Opt in! :cold_face:")
            .WithDescription("So you decided to opt in again, i like you you know?")
            .WithColor(Bot.DefaultEmbedColor);

        await ModifyOriginalResponseAsync(r => r.Embed = optInEmbed.Build());
    }

    [SlashCommand("out", "Opt out of the bot counter."), UsedImplicitly]
    public async Task OptOutCommand([Summary("delete", "Delete your NWord data.")]bool deleteData = false)
    {
        await DeferAsync(true);

        if (OptManager.UserOptedOut(Context.User.Id))
        {
            EmbedBuilder alreadyOptedOutEmbed = new EmbedBuilder()
                .WithTitle("You are out")
                .WithDescription("You can't opt out because you are already out, you can opt in again by running the </opt in:1125541780855722066> command.")
                .WithColor(Bot.DefaultEmbedColor);

            await ModifyOriginalResponseAsync(r => r.Embed = alreadyOptedOutEmbed.Build());
            return;
        }
        
        EmbedBuilder optOutEmbed = new EmbedBuilder()
            .WithTitle("Opt out")
            .WithDescription("Are you sure you want to opt out?")
            .AddField(":warning: Warning :warning:", "You have chosen to delete all of your personal bot data, your current stats won't be saved and aren't recoverable either!")
            .WithColor(0xFF0000);

        ComponentBuilder optOutComponents = new ComponentBuilder()
            .WithButton(new ButtonBuilder().WithLabel(deleteData ? "Only opt out" : "Opt out").WithStyle(ButtonStyle.Secondary).WithCustomId("ONLYOPTOUT"))
            .WithButton(new ButtonBuilder().WithLabel("Don't do anything").WithStyle(ButtonStyle.Secondary).WithCustomId("DONTDOANYTHING"));

        if (deleteData)
            optOutComponents = optOutComponents
                .WithButton(new ButtonBuilder().WithLabel("Delete all").WithStyle(ButtonStyle.Danger).WithCustomId("DELETEALLDATA"));

        await ModifyOriginalResponseAsync(r =>
        {
            r.Embed = optOutEmbed.Build();
            r.Components = optOutComponents.Build();
        });

        async Task OptOutMessageHandler(SocketMessageComponent component)
        {
            await component.DeferAsync(true);
            
            if (component.User.Id != Context.User.Id)
                return;
            
            if (component.Data.CustomId == "DONTDOANYTHING")
            {
                EmbedBuilder dontDoAnythingEmbed = new EmbedBuilder()
                    .WithTitle(":confetti_ball: Thanks for staying :confetti_ball:")
                    .WithDescription("Ahh, i was worried for a moment, thought you wanted to abandon me.")
                    .WithColor(Bot.DefaultEmbedColor);

                await ModifyOriginalResponseAsync(r =>
                {
                    r.Embed = dontDoAnythingEmbed.Build();
                    r.Components = new ComponentBuilder().Build();
                });
                return;
            }

            bool deleteDataConfirmation = component.Data.CustomId == "DELETEALLDATA";
            OptManager.OptOutUser(Context.User.Id, deleteDataConfirmation);

            EmbedBuilder deleteDataEmbed = new EmbedBuilder()
                .WithTitle(":wave: Good bye! :wave:")
                .WithDescription("You decided to opt out, sad to see you leave. You can always opt in again by running the </opt in:1125541780855722066> command.")
                .WithColor(Bot.DefaultEmbedColor);

            if (deleteDataConfirmation)
                deleteDataEmbed.AddField("Data deleted", "Your data was deleted by request.");

            await ModifyOriginalResponseAsync(r =>
            {
                r.Embed = deleteDataEmbed.Build();
                r.Components = new ComponentBuilder().Build();
            });
            
            Bot.Client.ButtonExecuted -= OptOutMessageHandler;
        }

        Bot.Client.ButtonExecuted += OptOutMessageHandler;
    }
}