using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
// note: the templateDatabase constructor argument type should be defined as an assembly fixture in any xunit test project using this class so that it will be provided automatically during instantiation
public sealed class SeededAppDatabase(SeededAppDatabaseTemplateBasedOnConfig databaseTemplate) : IDatabase
{
    private IDatabase? _database;
    private readonly SeededAppDatabaseTemplateBasedOnConfig _databaseTemplate = databaseTemplate ?? throw new ArgumentNullException(nameof(databaseTemplate));

    public string DatabaseName => _database?.DatabaseName ?? throw Exceptions.DatabaseNotInitialized();
    public DbConnection GetConnection() => _database?.GetConnection() ?? throw Exceptions.DatabaseNotInitialized();
    public string GetConnectionString() => _database?.GetConnectionString() ?? throw Exceptions.DatabaseNotInitialized();

    public DbProvider DbProvider => _databaseTemplate.DbProvider;

    public async ValueTask InitializeAsync()
    {
        _database = _databaseTemplate.NewDatabase();
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_database != null) await _database.DisposeAsync();
    }
}
