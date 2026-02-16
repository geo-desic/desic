using Desic.EntityFrameworkCore.DbConnections;
using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

// class is sealed for simpler IAsyncLifetime implementation
public sealed class DesicContextLocalDb : IAsyncLifetime
{
    private string? _connectionStringApp;
    private string? _connectionStringMigrations;
    private string? _databaseFilePath;
    private string? _databaseFileName;
    private string? _databaseName;
    private const string DataSource = @"(localdb)\MSSQLLocalDB";

    public string ConnectionStringApp => _connectionStringApp ?? throw new InvalidOperationException($"{nameof(ConnectionStringApp)} has not been initialized");
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw new InvalidOperationException($"{nameof(ConnectionStringMigrations)} has not been initialized");

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the localdb database
        _databaseName = $"desic_{Guid.NewGuid():N}";
        _databaseFileName = $"{_databaseName}.mdf";
        var tempDir = Path.Combine(Path.GetTempPath(), "desic-tests");
        Directory.CreateDirectory(tempDir);
        _databaseFilePath = Path.Combine(tempDir, _databaseFileName);

        _connectionStringMigrations = $"Data Source={DataSource};Initial Catalog={_databaseName};Integrated Security=True;AttachDbFilename={_databaseFilePath};";

        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", ConnectionStringMigrations]);

        using var cts = new CancellationTokenSource();
        await context.Database.MigrateAsync(cts.Token);

        var configKey = "Databases:Desic:AppUserPassword";
        var appUserPassword = TestSettingsConfiguration.Root.GetValue<string>(configKey);
        if (string.IsNullOrEmpty(appUserPassword)) throw new Exception($"Configuration value not set: {configKey}");

        _connectionStringApp = $"Data Source={DataSource};Initial Catalog={_databaseName};User ID={DesicContext.AppUser};Password={appUserPassword};";

        using var connection = new SqlConnection(_connectionStringApp);
        if (!await connection.CanConnectAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseFilePath))
        {
            using var connection = new SqlConnection($"Data Source={DataSource};Integrated Security=True;");
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"IF DB_ID('{_databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]; END";
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();

            try { File.Delete(_databaseFilePath); } catch { /* nothing */ }
            try { File.Delete(Path.ChangeExtension(_databaseFilePath, ".ldf")); } catch { /* nothing */ }
        }
    }
}
