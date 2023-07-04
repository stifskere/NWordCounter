namespace NwordCounter.Stuff;

public static class OptManager
{
    private static readonly List<ulong> OptedOutUsers = new();

    static OptManager()
    {
        DatabaseResult res = Bot.Database.Exec("SELECT user FROM OptedOutUsers");
        for (int i = 1; i <= res.RowCount; i++)
            OptedOutUsers.Add((ulong)res.GetValue<long>("user", i));
    }

    public static bool UserOptedOut(ulong user)
        => OptedOutUsers.Contains(user);

    public static bool OptInUser(ulong user)
    {
        if (!UserOptedOut(user))
            return false;
            
        OptedOutUsers.Remove(user);
        Bot.Database.Exec("DELETE FROM OptedOutUsers WHERE user = @user", ("user", user));
        return true;
    }

    public static bool OptOutUser(ulong user, bool deleteData)
    {
        if (UserOptedOut(user))
            return false;
            
        OptedOutUsers.Add(user);
        Bot.Database.Exec("INSERT INTO OptedOutUsers(user) VALUES(@user)", ("user", user));

        if (deleteData)
            Bot.Database.Exec("DELETE FROM UserNwords WHERE user = @user", ("user", user));

        return true;
    }
}