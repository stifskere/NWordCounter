namespace NwordCounter.Stuff;

public static class OptManager
{
    private static List<ulong> _optedOutUsers = new();

    static OptManager()
    {
        DatabaseResult res = Bot.Database.Exec("SELECT user FROM OptedOutUsers");
        for (int i = 1; i < res.RowCount + 1; i++)
            _optedOutUsers.Add((ulong)res.GetValue<long>("user", i));
    }

    public static bool UserOptedOut(ulong user)
        => _optedOutUsers.Contains(user);

    public static bool OptInUser(ulong user)
    {
        if (!_optedOutUsers.Contains(user))
            return false;
            
        _optedOutUsers.Remove(user);
        Bot.Database.Exec("DELETE FROM OptedOutUsers WHERE user = @user", ("user", user));
        return true;
    }

    public static bool OptOutUser(ulong user, bool deleteData)
    {
        if (_optedOutUsers.Contains(user))
            return false;
            
        _optedOutUsers.Add(user);
        Bot.Database.Exec("INSERT INTO OptedOutUsers(user) VALUES(@user)", ("user", user));

        if (deleteData)
            Bot.Database.Exec("DELETE FROM UserNwords WHERE user = @user", ("user", user));

        return true;
    }
}