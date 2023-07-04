
namespace NwordCounter.Stuff;

public class FileVariables
{
    private Dictionary<string, string> _variables = new();

    public FileVariables(string path)
    {
        if (!File.Exists(path))
            File.Create(path).Close();

        foreach (string line in File.ReadLines(path))
        {
            string[] separation = line.Split("=");
            
            if (separation.Length == 2)
                _variables.Add(separation[0], separation[1]);
        }
    }

    public string this[string key]
        => _variables[key];

    public bool EntryExists(string key) 
        => _variables.ContainsKey(key);
}