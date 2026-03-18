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
            if (_options?.DbProviders?.SqlServer?.UseContainer ?? false) // container
            {
                var image = _options?.DbProviders?.SqlServer?.ContainerImage ?? throw new InvalidOperationException("Container image for sql server is not configured");
                Console.WriteLine($"Using database: {DbProvider} (container) {image}");
                _database = new TemplateDatabaseSqlServerContainer(image: image);
            }
            else // local
            {
                var connectionStringInitialization = _options?.ConnectionStrings?.SqlServer ?? throw new InvalidOperationException("Connection string for database initialization could not be determined");
                var databaseInitializerOptions = _options?.Databases?.Application?.SqlServer?.Initialization ?? throw new InvalidOperationException($"Database initializer options for {DbProvider} is not configured");
                Console.WriteLine($"Using database: {DbProvider} (local)");
                _database = new TemplateDatabaseSqlServerLocal(connectionStringInitialization: connectionStringInitialization, databaseDirectoryPath: databaseDirectoryPath, options: databaseInitializerOptions);
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
