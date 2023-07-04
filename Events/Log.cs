using Discord;
using JetBrains.Annotations;
using NwordCounter.Stuff;

namespace NwordCounter.Events;

public static partial class Events
{
    [Event("Log"), UsedImplicitly]
    public static Task LogEvent(LogMessage message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}