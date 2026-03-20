using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

public interface ITestDatabaseSqlite : ITestDatabase
{
    SqliteConnection GetSqliteConnection();
    DbConnection ITestDatabase.GetConnection() => GetSqliteConnection();
    string DatabaseDirectoryPath { get; }
    string DatabaseFilePath { get; }
    string DatabaseFileName { get; }
    string DatabaseName { get; }
}
