namespace Desic.Testing.Integration.Db;

public sealed class TemplateDatabaseBasedOnConfig : ITemplateDatabase
{
    private ITemplateDatabase? _database;
    private readonly IntegrationTestsOptions? _options = TestConfiguration.Options;

    public ITestDatabase NewTestDatabase() => _database?.NewTestDatabase() ?? throw new InvalidOperationException(Constants.DatabaseNotInitialized);

    public async ValueTask InitializeAsync()
    {
        // make sure temporary directory for the database files exists
        var databaseDirectoryPath = Path.Combine(Path.GetTempPath(), $"{Constants.DatabaseName.ToLowerInvariant()}-tests");
        Directory.CreateDirectory(databaseDirectoryPath);

        if (DbProvider == DbProvider.Sqlite)
        {
            Console.WriteLine($"Using database: {DbProvider}");
            _database = new TemplateDatabaseSqlite(databaseDirectoryPath: databaseDirectoryPath);
        }
        else // sql server
        {
            var apiUserPassword = _options?.Databases?.Application?.SqlServer?.Users?["Api"]?.Password ?? throw new InvalidOperationException($"Api user {nameof(IntegrationTestsDatabaseApplicationSqlServerUsersOptions.Password)} is not configured");
            if (_options?.DbProviders?.SqlServer?.UseContainer ?? false)
            {
                var image = _options?.DbProviders?.SqlServer?.ContainerImage ?? throw new InvalidOperationException($"Container image for sql server is not configured");
                Console.WriteLine($"Using database: {DbProvider} (container) {image}");
                _database = new TemplateDatabaseSqlServerContainer(image: image, apiUserPassword: apiUserPassword);
            }
            else // localdb
            {
                Console.WriteLine($"Using database: {DbProvider} (localdb)");
                _database = new TemplateDatabaseLocalDb(databaseDirectoryPath: databaseDirectoryPath, apiUserPassword: apiUserPassword);
            }
        }
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _database?.DisposeAsync().AsTask().Wait();
    }

    public DbProvider DbProvider => _options?.DbProvider == DbProvider.Sqlite ? DbProvider.Sqlite : DbProvider.SqlServer;
}
