using System.Reflection;
using Discord.WebSocket;

namespace NwordCounter.Stuff;

[AttributeUsage(AttributeTargets.Method)]
public class Event : Attribute
{
    public string Name { get; }

    public Event(string name)
    {
        Name = name;
    }
    
    public static void RegisterEvents(DiscordSocketClient client, Assembly assembly)
    {
        foreach (IEnumerable<MethodInfo> classMethods in assembly.GetTypes().Select(t => t.GetMethods()))
        {
            foreach (MethodInfo method in classMethods)
            {
                if (method.GetCustomAttribute<Event>() is not { } ev) 
                    continue;
                
                if (typeof(DiscordSocketClient).GetEvent(ev.Name) is not { } info)
                    continue;
                
                info.AddEventHandler(client, method.CreateDelegate(info.EventHandlerType!));
            }
        }
    }
}