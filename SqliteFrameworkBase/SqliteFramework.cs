using System;
using System.IO;
using Mono.Data.Sqlite;
using Mono.Data.Sqlite.Orm;

namespace SqliteFrameworkBase;

public class SqliteFramework
{
    public static event Action<SqliteSession>? OnDbConnected; 

    public static SqliteSession? Session { get; private set; }

    public static void InitConnection(string path)
    {
        var dataSourceFile = $"Data Source={Path.GetFullPath(path)}; Version=3;";
        Session = new SqliteSession(dataSourceFile);
        OnDbConnected?.Invoke(Session);
    }

    public static void CloseConnection()
    {
        Session?.Dispose();
        Session = null;
    }

}