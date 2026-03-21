using Microsoft.Data.SqlClient;

namespace Desic.Testing.Integration.Db;

public sealed class EmptyDatabaseSqlServerContainer(bool contained = true, string databaseName = Constants.DatabaseName, string image = Containers.DefaultImageSqlServer) : IDatabaseSqlServer
{
    private string? _connectionString;
    private readonly SqlServerContainer _container = new(image);
    private readonly string _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
    private readonly string _image = image ?? throw new ArgumentNullException(nameof(image));

    public SqlConnection GetSqlServerConnection() => new(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseName => _databaseName;
    public string FromImage => _image;

    public async ValueTask InitializeAsync()
    {
        await _container.InitializeAsync();

        var connectionStringContainer = _container.GetConnectionString();

        await SqlServerOperations.CreateDatabase(connectionString: connectionStringContainer, contained: contained, databaseName: _databaseName);
        Console.Write($"Successfully created empty database [{_databaseName}] with contained = {contained}");

        _connectionString = new SqlConnectionStringBuilder(connectionStringContainer) { InitialCatalog = _databaseName }.ConnectionString;
        using var connection = GetSqlServerConnection();
        await connection.OpenAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
