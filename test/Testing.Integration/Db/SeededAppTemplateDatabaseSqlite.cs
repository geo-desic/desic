using Desic.Infrastructure.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppTemplateDatabaseSqlite(string databaseDirectoryPath) : ITemplateDatabase
{
    private readonly EmptyDatabaseSqlite _database = new(databaseDirectoryPath: databaseDirectoryPath, databaseNamePrefix: $"{Constants.DatabaseName.ToLowerInvariant()}_template");

    public ITestDatabase NewTestDatabase() => new SeededAppDatabaseSqlite(templateDatabaseFilePath: _database.DatabaseFilePath);

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();

        var connectionString = _database.GetConnectionString();

        // apply migrations and seed database
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", connectionString, "--environment", Constants.TestEnvironmentName]);

        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
    }
}
