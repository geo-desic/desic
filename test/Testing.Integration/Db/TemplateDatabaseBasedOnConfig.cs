using Desic.Infrastructure.Data.SqlServer;

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
            var databaseInitializerOptions = _options?.Databases?.Application?.SqlServer ?? throw new InvalidOperationException($"Database initializer options for {DbProvider} is not configured");
            if (_options?.DbProviders?.SqlServer?.UseContainer ?? false) // container
            {
                var apiUserPassword = databaseInitializerOptions.Users?.Api?.Password ?? throw new InvalidOperationException($"{nameof(DatabaseInitializerUsersOptions.Api)} user {nameof(DatabaseInitializerUserOptions.Password)} is not configured");
                var image = _options?.DbProviders?.SqlServer?.ContainerImage ?? throw new InvalidOperationException("Container image for sql server is not configured");
                Console.WriteLine($"Using database: {DbProvider} (container) {image}");
                _database = new TemplateDatabaseSqlServerContainer(image: image, apiUserPassword: apiUserPassword);
            }
            else // local
            {
                var connectionStringInitialization = _options?.ConnectionStrings?.SqlServer ?? throw new InvalidOperationException($"Connection string {nameof(IntegrationTestsConnectionStringsOptions.SqlServer)} is not configured");
                Console.WriteLine($"Using database: {DbProvider} (local)");
                _database = new TemplateDatabaseSqlServerLocal(connectionStringInitialization: connectionStringInitialization, databaseDirectoryPath: databaseDirectoryPath, databaseInitializerOptions: databaseInitializerOptions);
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
