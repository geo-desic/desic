using Desic.Api.Db;
using Desic.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simpler IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TestDatabaseLocalDb(string databaseDirectoryPath, string templateDatabaseName, string templateDatabaseBackupFilePath, string apiUserPassword) : ITestDatabase
{
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");
    private string? _connectionString;
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    private string? _databaseFilePath;
    private string? _databaseLogFilePath;
    private string? _databaseName;
    private readonly string _templateDatabaseBackupFilePath = templateDatabaseBackupFilePath ?? throw new ArgumentNullException(nameof(templateDatabaseBackupFilePath));
    private readonly string _templateDatabaseName = templateDatabaseName ?? throw new ArgumentNullException( nameof(templateDatabaseName));

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the database
        _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFilePath = Path.Combine(_databaseDirectoryPath, $"{_databaseName}.mdf");
        _databaseLogFilePath = Path.Combine(_databaseDirectoryPath, $"{_databaseName}.ldf");

        await RestoreDatabase();
        Console.Write($"Successfully restored database {_databaseName} using {_templateDatabaseBackupFilePath}");

        _connectionString = $"Data Source={TemplateDatabaseLocalDb.DataSource};Initial Catalog={_databaseName};User ID={Providers.DbApiUser};Password={_apiUserPassword};";

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseFilePath))
        {
            using var connection = new SqlConnection($"Data Source={TemplateDatabaseLocalDb.DataSource};Integrated Security=True;");
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"IF DB_ID('{_databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]; END";
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();

            try { File.Delete(_databaseFilePath); } catch { /* nothing */ }
        }
        if (!string.IsNullOrEmpty(_databaseLogFilePath))
        {
            try { File.Delete(_databaseLogFilePath); } catch { /* nothing */ }
        }
    }

    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw NewDatabaseNotInitializedException());

    public string GetConnectionString() => _connectionString ?? throw NewDatabaseNotInitializedException();

    private static InvalidOperationException NewDatabaseNotInitializedException() => new(Constants.DatabaseNotInitialized);

    private async Task RestoreDatabase()
    {
        using var connection = new SqlConnection($"Data Source={TemplateDatabaseLocalDb.DataSource};Integrated Security=True;");
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"RESTORE DATABASE [{_databaseName}] FROM DISK = N'{_templateDatabaseBackupFilePath}' WITH MOVE '{_templateDatabaseName}' TO '{_databaseFilePath}', MOVE '{_templateDatabaseName}_Log' TO '{_databaseLogFilePath}', RECOVERY, REPLACE;";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }
}
