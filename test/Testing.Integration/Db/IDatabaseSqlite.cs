using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

public interface IDatabaseSqlite : IDatabase
{
    SqliteConnection GetSqliteConnection();
    DbConnection IDatabaseServer.GetConnection() => GetSqliteConnection();
    string DatabaseDirectoryPath { get; }
    string DatabaseFilePath { get; }
    string DatabaseFileName { get; }
}
