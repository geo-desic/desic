using Desic.Core.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Data;

namespace Desic.EntityFrameworkCore.SqlServer;

public class DatabaseInitializer(IOptions<DatabaseInitializerOptions> options, ILogger<DatabaseInitializer> logger)
{
    private string? _databaseName;
    private readonly DatabaseInitializerOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<DatabaseInitializer> _logger = logger ?? NullLogger<DatabaseInitializer>.Instance;

    public async Task InitializeAsync(string connectionString, string? targetDatabaseName = null, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(connectionString);
        await InitializeAsync(connection, targetDatabaseName, cancellationToken);
    }

    public async Task InitializeAsync(SqlConnection connection, string? targetDatabaseName = null, CancellationToken cancellationToken = default)
    {
        _databaseName = targetDatabaseName ?? _options.Name ?? throw new InvalidOperationException("Database name is not specified in options or method parameters");

        if (connection.State == ConnectionState.Closed && !await connection.CanConnectAsync(cancellationToken))
        {
            connection.ConnectionString = SqlConnectionHelpers.UpdateDatabaseInConnectionString(connection.ConnectionString, "master");

            if (!await connection.CanConnectAsync(cancellationToken))
            {
                _logger.LogError("Cannot connect to the database server with the provided connection string");
                throw new InvalidOperationException("Cannot connect to the database server with the provided connection string");
            }
        }

        if (await DatabaseExistsAsync(connection, cancellationToken))
        {
            if (_options.StopIfExists ?? false)
            {
                _logger.LogInformation($"Stopping as the database already exists and '{nameof(_options.StopIfExists)}' is true");
                return;
            }
        }
        else
        {
            await CreateDatabaseAsync(connection, cancellationToken);
        }

        // ensure we are connected to the new database before proceeding
        await connection.ChangeDatabaseAsync(_databaseName, cancellationToken);

        await CreateSchemasAsync(connection, cancellationToken);

        await CreateRolesAsync(connection, cancellationToken);

        await CreateUsersAsync(connection, cancellationToken);
    }

    #region Database
    private async Task<bool> CreateDatabaseAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        var sql = $"CREATE DATABASE [{_databaseName}]";
        if (_options.Contained ?? true) sql += " CONTAINMENT = PARTIAL";
        using (var command = new SqlCommand(sql, connection))
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("Database created: '{DatabaseName}'", _databaseName);
        return true;
    }

    private async Task<bool> DatabaseExistsAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_databaseName)) throw new InvalidOperationException("Database name is not specified");
        using var command = new SqlCommand("SELECT DB_ID(@databaseName)", connection).AddParameterWithValue("@databaseName", _databaseName);
        var result = await command.ExecuteScalarAsyncAs<int>(cancellationToken);
        return result != null;
    }
    #endregion

    #region Logins
    private async Task<bool> CreateLoginAsync(SqlConnection connection, string? name, string? password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Login name is not specified");
        if (await LoginExistsAsync(connection, name, cancellationToken))
        {
            _logger.LogDebug("Login already exists: {LoginName}", name);
            return false;
        }
        if (string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException("Login password is not specified");
        var sql = $"CREATE LOGIN [{name}] WITH PASSWORD = '{password}'";
        using (var command = new SqlCommand(sql, connection))
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("Login created: '{LoginName}'", name);
        return true;
    }

    private static async Task<bool> LoginExistsAsync(SqlConnection connection, string loginName, CancellationToken cancellationToken)
    {
        using var command = new SqlCommand("SELECT SUSER_ID(@loginName)", connection).AddParameterWithValue("@loginName", loginName);
        var result = await command.ExecuteScalarAsyncAs<int>(cancellationToken);
        return result != null;
    }
    #endregion

    #region Schemas
    private async Task<bool> CreateSchemaAsync(SqlConnection connection, DatabaseInitializerSchemaOptions schema, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(schema.Name)) throw new InvalidOperationException("Schema name is not specified");
        if (await SchemaExistsAsync(connection, schema.Name, cancellationToken))
        {
            _logger.LogDebug("Schema already exists: {SchemaName}", schema.Name);
            return false;
        }
        var sql = $"CREATE SCHEMA [{schema.Name}]";
        if (!string.IsNullOrWhiteSpace(schema.AuthorizationOwnerName)) sql += $" AUTHORIZATION [{schema.AuthorizationOwnerName}]";
        using (var command = new SqlCommand(sql, connection))
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("Schema created: '{SchemaName}'", schema.Name);
        return true;
    }

    private async Task<int> CreateSchemasAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        var result = 0;
        foreach (var schema in _options.Schemas ?? [])
        {
            await CreateSchemaAsync(connection, schema, cancellationToken);
            ++result;
        }
        return result;
    }

    private static async Task<bool> SchemaExistsAsync(SqlConnection connection, string schemaName, CancellationToken cancellationToken)
    {
        using var command = new SqlCommand("SELECT SCHEMA_ID(@schemaName)", connection).AddParameterWithValue("@schemaName", schemaName);
        var result = await command.ExecuteScalarAsyncAs<int>(cancellationToken);
        return result != null;
    }
    #endregion

    #region Roles
    private async Task<int> CreateRolesAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        var result = 0;
        foreach (var role in _options.Roles ?? [])
        {
            await CreateRoleAsync(connection, role, cancellationToken);
            ++result;
            foreach (var grant in role.Grants ?? [])
            {
                await CreateRoleGrantsAsync(connection, grant, role.Name!, cancellationToken);
            }
        }
        return result;
    }

    private async Task<bool> CreateRoleAsync(SqlConnection connection, DatabaseInitializerRoleOptions role, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(role.Name)) throw new InvalidOperationException("Role name is not specified");
        if (await RoleExistsAsync(connection, role.Name, cancellationToken))
        {
            _logger.LogDebug("Role already exists: {RoleName}", role.Name);
            return false;
        }
        var sql = $"CREATE ROLE [{role.Name}]";
        if (!string.IsNullOrWhiteSpace(role.AuthorizationOwnerName)) sql += $" AUTHORIZATION [{role.AuthorizationOwnerName}]";
        using (var command = new SqlCommand(sql, connection))
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("Role created: '{RoleName}'", role.Name);
        return true;
    }

    private async Task<bool> CreateRoleGrantsAsync(SqlConnection connection, DatabaseInitializerRoleGrantOptions grant, string roleName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(grant.Schema)) throw new InvalidOperationException("Grant schema name is not specified");
        var permissions = grant.Permissions != null && grant.Permissions.Count > 0
            ? string.Join(", ", grant.Permissions.Select(p => $"{p}"))
            : "*";
        var grantSql = $"GRANT {permissions} ON SCHEMA::[{grant.Schema}] TO [{roleName}]";
        using (var grantCommand = new SqlCommand(grantSql, connection))
        {
            await grantCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("Granted permissions '{Permissions}' on schema '{SchemaName}' to role '{RoleName}'", permissions, grant.Schema, roleName);
        return true;
    }

    private static async Task<bool> RoleExistsAsync(SqlConnection connection, string roleName, CancellationToken cancellationToken)
    {
        using var command = new SqlCommand("SELECT DATABASE_PRINCIPAL_ID(@roleName)", connection).AddParameterWithValue("@roleName", roleName);
        var result = await command.ExecuteScalarAsyncAs<int>(cancellationToken);
        return result != null;
    }
    #endregion

    #region Users
    private async Task<bool> AddUserToRole(SqlConnection connection, string userName, string? roleName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(roleName)) throw new InvalidOperationException("User role name is not specified");
        var addRoleSql = $"ALTER ROLE [{roleName}] ADD MEMBER [{userName}]";
        using (var addRoleCommand = new SqlCommand(addRoleSql, connection))
        {
            await addRoleCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("User '{UserName}' added to role '{RoleName}'", userName, roleName);
        return true;
    }

    private async Task<int> CreateUsersAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        var result = 0;
        foreach (var user in _options.Users ?? [])
        {
            await CreateUserAsync(connection, user, cancellationToken);
            ++result;
            foreach (var role in user.Roles ?? [])
            {
                await AddUserToRole(connection, user.Name!, role, cancellationToken);
            }
        }
        return result;
    }

    private async Task<bool> CreateUserAsync(SqlConnection connection, DatabaseInitializerUserOptions user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Name)) throw new InvalidOperationException("User name is not specified");
        var contained = _options.Contained ?? true;
        if (!contained)
        {
            await CreateLoginAsync(connection, user.Name, user.Password, cancellationToken);
        }
        if (await UserExistsAsync(connection, user.Name, cancellationToken))
        {
            _logger.LogDebug("User already exists: {UserName}", user.Name);
            return false;
        }
        var sql = $"CREATE USER [{user.Name}]";
        if (contained)
        {
            if (string.IsNullOrWhiteSpace(user.Password)) throw new InvalidOperationException("User password is not specified");
            sql += $" WITH PASSWORD = '{user.Password}'";
            if (!string.IsNullOrWhiteSpace(user.DefaultSchema)) sql += $", DEFAULT_SCHEMA = [{user.DefaultSchema}]";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(user.LoginName)) throw new InvalidOperationException("Login name must be specified when Contained is false");
            sql += $" FOR LOGIN [{user.LoginName}]";
            if (!string.IsNullOrWhiteSpace(user.DefaultSchema)) sql += $" WITH DEFAULT_SCHEMA = [{user.DefaultSchema}]";
        }
        using (var command = new SqlCommand(sql, connection))
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        _logger.LogInformation("User created: '{UserName}'", user.Name);
        return true;
    }

    private static async Task<bool> UserExistsAsync(SqlConnection connection, string userName, CancellationToken cancellationToken)
    {
        using var command = new SqlCommand("SELECT USER_ID(@userName)", connection).AddParameterWithValue("@userName", userName);
        var result = await command.ExecuteScalarAsyncAs<int>(cancellationToken);
        return result != null;
    }
    #endregion
}
