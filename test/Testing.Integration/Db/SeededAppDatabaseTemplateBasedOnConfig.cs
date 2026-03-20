namespace Desic.Testing.Integration.Db;

public sealed class SeededAppDatabaseTemplateBasedOnConfig : IDatabaseTemplate
{
    private IDatabaseTemplate? _database;
    private readonly IntegrationTestsOptions? _options = TestConfiguration.Options;

    public DbProvider DbProvider => _options?.DbProvider == DbProvider.Sqlite ? DbProvider.Sqlite : DbProvider.SqlServer;
    public IDatabase NewDatabase() => _database?.NewDatabase() ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        // make sure temporary directory for the database files exists
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), Constants.TestsTempDirectoryName);
        Directory.CreateDirectory(tempDirectoryPath);

        if (DbProvider == DbProvider.Sqlite)
        {
            Console.WriteLine($"Using database: {DbProvider}");
            _database = new SeededAppDatabaseTemplateSqlite(databaseDirectoryPath: tempDirectoryPath);
        }
        else // sql server
        {
            if (_options?.DbProviders?.SqlServer?.UseContainer ?? false) // container
            {
                var image = _options?.DbProviders?.SqlServer?.ContainerImage ?? Containers.DefaultImageSqlServer;
                Console.WriteLine($"Using database: {DbProvider} (container) {image}");
                _database = new SeededAppDatabaseTemplateSqlServerContainer(image: image);
            }
            else // local
            {
                var connectionStringInitialization = _options?.ConnectionStrings?.SqlServer ?? throw new InvalidOperationException("Connection string for database initialization could not be determined");
                var databaseInitializerOptions = _options?.Databases?.Application?.SqlServer?.Initialization ?? throw new InvalidOperationException($"Database initializer options for {DbProvider} is not configured");
                Console.WriteLine($"Using database: {DbProvider} (local)");
                _database = new SeededAppDatabaseTemplateSqlServerLocal(connectionStringInitialization: connectionStringInitialization, backupDirectoryPath: tempDirectoryPath, options: databaseInitializerOptions);
            }
        }
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_database != null) await _database.DisposeAsync();
    }
}
