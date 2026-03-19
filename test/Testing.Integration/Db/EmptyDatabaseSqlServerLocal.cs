using Desic.Shared.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simpler IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class EmptyDatabaseSqlServerLocal : ITestDatabase
{
    private readonly string _connectionString;
    private readonly bool _contained;
    private readonly string _connectionStringDatabase;
    private readonly string _databaseName;

    public EmptyDatabaseSqlServerLocal(string connectionString, bool contained = true, string? databaseNamePrefix = null)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _contained = contained;
        _databaseName = $"{databaseNamePrefix ?? Constants.DatabaseName.ToLowerInvariant()}_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _connectionStringDatabase = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = _databaseName }.ConnectionString;
    }

    public DbConnection GetConnection() => new SqlConnection(_connectionStringDatabase ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionStringDatabase ?? throw Exceptions.DatabaseNotInitialized();
    public string GetConnectionStringMaster() => _connectionString;
    public string DatabaseName => _databaseName;

    public async ValueTask InitializeAsync()
    {
        await SqlServerOperations.CreateDatabase(connectionString: _connectionString, contained: _contained, databaseName: _databaseName);
        Console.Write($"Successfully created empty database [{_databaseName}] with contained = {_contained}");

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the local empty database");
    }

    public async ValueTask DisposeAsync()
    {
        await SqlServerOperations.DropDatabase(connectionString: _connectionString, databaseName: _databaseName);
    }
}
