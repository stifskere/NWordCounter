using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace NwordCounter.Stuff;

[PublicAPI]
public abstract class DatabaseResult : IEnumerable<object[]>
{
    protected abstract KeyValuePair<string, int>[] Indexes { get; set; }
    protected abstract object[][] Values { get; set; }
    public bool HasResult => RowCount > 0;
    public int ColumnCount => Indexes.Length;
    public int RowCount => Values.Length;
    
    public object this[string column, int row] 
        => row > 0 && row < RowCount + 1 ? 
            Values[row - 1][Indexes.First(i => i.Key == column).Value] :
            throw new ArgumentException(row < RowCount + 1 ? 
                "Row may not be smaller than 1." : 
                "Row may not be bigger than the number of rows returned.");

    public T GetValue<T>(string column, int row)
        => (T)this[column, row];

    public IEnumerator<object[]> GetEnumerator() 
        => ((IEnumerable<object[]>)Values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}

[PublicAPI]
public class DatabaseHandler
{
    private readonly SQLiteConnection _con;
    
    public DatabaseHandler(string path)
    {
        string folderPath = string.Join('/', Regex.Split(path, @"[/\\]", RegexOptions.Multiline)[..^1]);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(string.Join('/', folderPath));
        if(!File.Exists(path)) SQLiteConnection.CreateFile(path);
        _con = new SQLiteConnection($"URI=file:{path}");
        _con.Open();
    }

    ~DatabaseHandler() => _con.Dispose();
    public DatabaseResult Exec(string command, params (string Name, object Value)[] parameters)
    {
        SQLiteCommand cmd = new SQLiteCommand(command, _con);

        for (int i = 0; i < parameters.Length; i++)
        {
            cmd.Parameters.Add($"@{parameters[i].Name}", parameters[i].Value switch
            {
                string => DbType.String,
                int => DbType.Int32,
                long => DbType.Int64,
                short => DbType.Int16,
                bool or byte => DbType.Byte,
                double or float => DbType.Double,
                decimal => DbType.Decimal,
                _ => DbType.Object
            });
            cmd.Parameters[$"@{parameters[i].Name}"].Value = parameters[i].Value;
        }
        
        SQLiteDataReader reader = cmd.ExecuteReader();

        if (!reader.HasRows)
            return new DatabaseResultImpl();

        string[] columnNames = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columnNames[i] = reader.GetName(i);

        List<object[]> rows = new();
        while (reader.Read())
        {
            object[] values = new object[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                values[i] = reader.GetValue(i);
            rows.Add(values);
        }

        return new DatabaseResultImpl(columnNames, rows.ToArray());
    }

    private class DatabaseResultImpl : DatabaseResult
    {
        protected sealed override KeyValuePair<string, int>[] Indexes { get; set; }
        protected sealed override object[][] Values { get; set; }

        public DatabaseResultImpl()
        {
            Indexes = Array.Empty<KeyValuePair<string, int>>();
            Values = Array.Empty<object[]>();
        }
        
        public DatabaseResultImpl(string[] columnNames, object[][] rows)
        {
            Indexes = new KeyValuePair<string, int>[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
                Indexes[i] = new KeyValuePair<string, int>(columnNames[i], i);

            Values = new object[rows.Length][];
            for (int i = 0; i < rows.Length; i++)
            {
                Values[i] = new object[columnNames.Length];
                Values[i] = rows[i];
            }
        }
    }
}