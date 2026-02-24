using Desic.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Desic.Infrastructure.SqlServer;

public class DatabaseInitializer(IOptions<DatabaseInitializerOptions> options, ILogger<DatabaseInitializer> logger)
{
    private bool _contained;
    private string? _databaseName;
    private readonly DatabaseInitializerOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<DatabaseInitializer> _logger = logger ?? NullLogger<DatabaseInitializer>.Instance;

    public async Task InitializeAsync(string connectionString, string? targetDatabaseName = null, CancellationToken cancellationToken = default)
    {
        _databaseName = targetDatabaseName ?? _options.Name ?? throw new InvalidOperationException("Database name is not specified in options or method parameters");

        using var connection = await GetConnection(connectionString, cancellationToken);
        var serverConnection = new ServerConnection(connection);
        var server = new Server(serverConnection);

        var containmentEnabled = server.Configuration.ContainmentEnabled.ConfigValue == 1;

        // default containment is true if server has it enabled or false otherwise
        _contained = _options.Contained ?? containmentEnabled;

        if (server.Databases.Contains(_databaseName))
        {
            if (_options.StopIfExists ?? false)
            {
                _logger.LogInformation($"Stopping as the database already exists and '{nameof(_options.StopIfExists)}' is true");
                return;
            }
            if (_contained && !containmentEnabled) SetContainment(server);
        }
        else
        {
            if (_contained && !containmentEnabled) SetContainment(server);
            CreateDatabase(server, _databaseName, _contained);
            _logger.LogInformation("Database created: '{DatabaseName} with containment = {IsContained}'", _databaseName, _contained);
        }

        var database = server.Databases[_databaseName];

        // ensure we are connected to the new database before proceeding
        await connection.ChangeDatabaseAsync(_databaseName, cancellationToken);

        CreateSchemas(database);

        CreateRoles(database);

        CreateUsers(server, database);
    }

    private async Task<SqlConnection> GetConnection(string connectionString, CancellationToken cancellationToken)
    {
        var builder = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = 5 };
        var connection = new SqlConnection(builder.ConnectionString);
        if (!await connection.CanConnectAsync(cancellationToken))
        {
            await connection.DisposeAsync();
            // possible the database does not exist, so try to connect to master database
            builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master", ConnectTimeout = 5 };
            var connectionMaster = new SqlConnection(builder.ConnectionString);

            if (!await connectionMaster.CanConnectAsync(cancellationToken))
            {
                _logger.LogError("Cannot connect to the database server with the provided connection string");
                throw new InvalidOperationException("Cannot connect to the database server with the provided connection string");
            }
            return connectionMaster;
        }
        return connection;
    }

    #region Containment
    private void SetContainment(Server server)
    {
        server.Configuration.ContainmentEnabled.ConfigValue = 1;
        server.Configuration.Alter();
        _logger.LogInformation("Enabled contained dataabse authentication for the server");
    }
    #endregion

    #region Database
    private static void CreateDatabase(Server server, string name, bool contained)
    {
        var database = new Database(server, name)
        {
            ContainmentType = contained ? ContainmentType.Partial : ContainmentType.None
        };

        database.Create();
    }
    #endregion

    #region Logins
    private void CreateLogin(Server server, string name, string? password, string? defaultDatabase = null)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException("User password is not specified");
        var login = server.Logins[name];
        if (login != null)
        {
            _logger.LogDebug("Login exists: {UserName}", login.Name);
            return;
        }
        login = new Login(server, name);
        login.LoginType = LoginType.SqlLogin;
        if (defaultDatabase != null) login.DefaultDatabase = defaultDatabase;
        login.Create(password);
        login.Enable();
    }
    #endregion

    #region Schemas
    private void CreateSchema(Database database, DatabaseInitializerSchemaOptions schemaOptions)
    {
        if (string.IsNullOrWhiteSpace(schemaOptions.Name)) throw new InvalidOperationException("Schema name is not specified");

        if (database.Schemas[schemaOptions.Name] != null)
        {
            _logger.LogDebug("Schema already exists: {SchemaName}", schemaOptions.Name);
            return;
        }

        var schema = new Schema(database, schemaOptions.Name);
        if (!string.IsNullOrWhiteSpace(schemaOptions.OwnerName)) { schema.Owner = schemaOptions.OwnerName; }

        schema.Create();

        _logger.LogInformation("Schema created: '{SchemaName}'", schema.Name);
    }

    private void CreateSchemas(Database database)
    {
        foreach (var schema in _options.Schemas ?? [])
        {
            CreateSchema(database, schema);
        }
    }
    #endregion

    #region Roles
    private void CreateRoles(Database database)
    {
        foreach (var roleOptions in _options.Roles ?? [])
        {
            var role = CreateRole(database, roleOptions);
            foreach (var grant in roleOptions.Grants ?? [])
            {
                CreateRoleGrants(database, grant, role);
            }
        }
    }

    private DatabaseRole CreateRole(Database database, DatabaseInitializerRoleOptions roleOptions)
    {
        if (string.IsNullOrWhiteSpace(roleOptions.Name)) throw new InvalidOperationException("Role name is not specified");
        var existingRole = database.Roles[roleOptions.Name];
        if (existingRole != null)
        {
            _logger.LogDebug("Role already exists: {RoleName}", roleOptions.Name);
            return existingRole;
        }
        var role = new DatabaseRole(database, roleOptions.Name);
        if (roleOptions.OwnerName != null) { role.Owner = roleOptions.OwnerName; }
        role.Create();
        _logger.LogInformation("Role created: '{RoleName}'", role.Name);
        return role;
    }

    private void CreateRoleGrants(Database database, DatabaseInitializerRoleGrantOptions grant, DatabaseRole role)
    {
        if (string.IsNullOrWhiteSpace(grant.Schema)) throw new InvalidOperationException("Grant schema name is not specified");
        var permissions = grant.Permissions ?? [];
        var permissionSet = new ObjectPermissionSet
        {
            Select = permissions.Contains("SELECT", StringComparer.OrdinalIgnoreCase),
            Insert = permissions.Contains("INSERT", StringComparer.OrdinalIgnoreCase),
            Update = permissions.Contains("UPDATE", StringComparer.OrdinalIgnoreCase),
            Delete = permissions.Contains("DELETE", StringComparer.OrdinalIgnoreCase),
        };
        database.Schemas[grant.Schema].Grant(permissionSet, role.Name);
        _logger.LogInformation("Granted permissions on schema '{SchemaName}' to role '{RoleName}'", grant.Schema, role.Name);
    }
    #endregion

    #region Users
    private void AddUserToRole(Database database, string userName, string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName)) throw new InvalidOperationException("User role name is not specified");
        var role = database.Roles[roleName];
        role.AddMember(userName);
        _logger.LogInformation("User '{UserName}' added to role '{RoleName}'", userName, roleName);
    }

    private void CreateUsers(Server server, Database database)
    {
        foreach (var user in _options.Users ?? [])
        {
            CreateUser(server, database, user);
            foreach (var role in user.Roles ?? [])
            {
                AddUserToRole(database, user.Name!, role);
            }
        }
    }

    private void CreateUser(Server server, Database database, DatabaseInitializerUserOptions userOptions)
    {
        if (string.IsNullOrWhiteSpace(userOptions.Name)) throw new InvalidOperationException("User name is not specified");
        if (!_contained)
        {
            CreateLogin(server, userOptions.Name, userOptions.Password);
        }
        var user = database.Users[userOptions.Name];
        if (user != null)
        {
            _logger.LogDebug("User already exists: {UserName}", userOptions.Name);
            return;
        }
        user = new User(database, userOptions.Name);
        if (!string.IsNullOrWhiteSpace(userOptions.DefaultSchema)) { user.DefaultSchema = userOptions.DefaultSchema; }

        if (_contained)
        {
            user.Create(userOptions.Password);
        }
        else
        {
            user.Login = user.Name;
            user.Create();
        }
        _logger.LogInformation("User created: '{UserName}'", userOptions.Name);
    }
    #endregion
}
