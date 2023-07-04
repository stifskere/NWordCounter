using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using NwordCounter.Stuff;

namespace NwordCounter.Events;

public static partial class Events
{
    [Event("Ready"), UsedImplicitly]
    public static async Task ReadyEvent()
    {
        InteractionService iService = new(Bot.Client);
        await iService.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
        await iService.RegisterCommandsGloballyAsync();

        Bot.Client.InteractionCreated += async interaction
            => await iService.ExecuteCommandAsync(new SocketInteractionContext(Bot.Client, interaction), null);
        
        iService.SlashCommandExecuted += (_, _, result) => { 
            if (!result.IsSuccess) Console.WriteLine(result);
            return Task.CompletedTask;
        };

        Bot.Database.Exec(@"CREATE TABLE IF NOT EXISTS UserNwords(user INTEGER, guild INTEGER, count INTEGER, normalCount INTEGER, CONSTRAINT unq UNIQUE(user, guild));
                                    CREATE TABLE IF NOT EXISTS GuildNwords(guild INTEGER, count INTEGER, UNIQUE(guild));
                                    CREATE TABLE IF NOT EXISTS OptedOutUsers(user INTEGER, UNIQUE(user))");

        [DoesNotReturn]
        async Task ActivityLoop()
        {
            Game[] activities =
            {
                new("for that specific word.", ActivityType.Watching),
                new("how you say that word.", ActivityType.Listening),
                new("the word.", ActivityType.Watching),
                new("???!!???!?!?!??!?!")
            };
            
            while (true)
            {
                foreach (Game activity in activities)
                {
                    await Bot.Client.SetActivityAsync(activity);
                    await Task.Delay(TimeSpan.FromHours(1));
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
        
        new Thread(() => ActivityLoop().GetAwaiter().GetResult()).Start();

        Bot.Client.Ready -= ReadyEvent;
    }
}