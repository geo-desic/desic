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
        // create a unique file for the LocalDB database
        _databaseName = $"desic_{Guid.NewGuid():N}";
        _databaseFileName = $"{_databaseName}.mdf";
        var tempDir = Path.Combine(Path.GetTempPath(), "desic-tests");
        Directory.CreateDirectory(tempDir);
        _databaseFilePath = Path.Combine(tempDir, _databaseFileName);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = DataSource,
            InitialCatalog = _databaseName,
            IntegratedSecurity = true,
            AttachDBFilename = _databaseFilePath,
        };

        _connectionStringMigrations = builder.ConnectionString;

        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", ConnectionStringMigrations]);

        using var cts = new CancellationTokenSource();
        await context.Database.MigrateAsync(cts.Token);

        var configKey = "Databases:Desic:AppUserPassword";
        var appUserPassword = TestSettingsConfiguration.Root.GetValue<string>(configKey);
        if (string.IsNullOrEmpty(appUserPassword)) throw new Exception($"Configuration value not set: {configKey}");

        var appBuilder = new SqlConnectionStringBuilder(ConnectionStringMigrations)
        {
            DataSource = DataSource,
            InitialCatalog = _databaseName,
            AttachDBFilename = _databaseFilePath,
            UserID = DesicContext.AppUser,
            Password = appUserPassword,
        };

        _connectionStringApp = appBuilder.ConnectionString;
    }

    public async ValueTask DisposeAsync()
    {
        // attempt to detach and remove the database file
        try
        {
            if (!string.IsNullOrEmpty(_databaseFilePath))
            {
                using var connection = new SqlConnection(new SqlConnectionStringBuilder { DataSource = DataSource, IntegratedSecurity = true }.ConnectionString);
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"IF DB_ID('{_databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]; END";
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();

                try { File.Delete(_databaseFilePath); } catch { }
                try { File.Delete(Path.ChangeExtension(_databaseFilePath, ".ldf")); } catch { }
            }
        }
        catch
        {
            // swallow any exception during cleanup to avoid breaking test teardown
        }
    }
}
