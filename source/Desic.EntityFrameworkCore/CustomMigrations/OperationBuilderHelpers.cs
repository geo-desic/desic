using Desic.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Desic.EntityFrameworkCore.CustomMigrations;

public static class OperationBuilderHelpers
{
    private const string AppRole = $"rl_{DesicContext.AppUser}";
    private const string AppUserLogin = $"{DesicContext.AppUser}";

    public static void CreateAppUserAndPermissions(this MigrationBuilder migrationBuilder, string password)
    {
        switch (migrationBuilder.ActiveProvider)
        {
            case ProviderNames.Sqlite: return; // does not support users
            /*
            case ProviderNames.PostgreSQL:
                {
                    migrationBuilder.Sql($"CREATE USER {DesicContext.AppUser} WITH PASSWORD '{password}';");
                    // \c DesicContext
                    migrationBuilder.Sql($"REVOKE CREATE ON SCHEMA public FROM PUBLIC;");
                    migrationBuilder.Sql($"GRANT USAGE ON SCHEMA {DesicContext.AppSchema} TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA {DesicContext.AppSchema} TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"GRANT USAGE ON ALL SEQUENCES IN {DesicContext.AppSchema} TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA {DesicContext.AppSchema} GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"GRANT USAGE ON SCHEMA {DesicContext.RefSchema} TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"GRANT SELECT ON ALL TABLES IN SCHEMA {DesicContext.RefSchema} TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA {DesicContext.RefSchema} GRANT SELECT ON TABLES TO {DesicContext.AppUser};");
                    migrationBuilder.Sql($"ALTER USER {DesicContext.AppUser} SET search_path = {DesicContext.AppSchema};");
                    return;
                }
            */
            case ProviderNames.SqlServer:
                {
                    migrationBuilder.Sql($"CREATE LOGIN [{AppUserLogin}] WITH PASSWORD = N'{password}';");
                    migrationBuilder.Sql($"CREATE USER [{DesicContext.AppUser}] FOR LOGIN [{AppUserLogin}];");
                    migrationBuilder.Sql($"CREATE ROLE [{AppRole}];");
                    migrationBuilder.Sql($"GRANT SELECT ON SCHEMA::[{DesicContext.RefSchema}] TO [{AppRole}];");
                    migrationBuilder.Sql($"GRANT SELECT ON SCHEMA::[{DesicContext.AppSchema}] TO [{AppRole}];");
                    migrationBuilder.Sql($"GRANT INSERT ON SCHEMA::[{DesicContext.AppSchema}] TO [{AppRole}];");
                    migrationBuilder.Sql($"GRANT UPDATE ON SCHEMA::[{DesicContext.AppSchema}] TO [{AppRole}];");
                    migrationBuilder.Sql($"GRANT DELETE ON SCHEMA::[{DesicContext.AppSchema}] TO [{AppRole}];");
                    migrationBuilder.Sql($"ALTER ROLE [{AppRole}] ADD MEMBER [{DesicContext.AppUser}];");
                    return;
                }
        }
        throw new Exception($"Provider is unsupported: {migrationBuilder.ActiveProvider}");
    }

    public static void UndoCreateAppUserAndPermissions(this MigrationBuilder migrationBuilder)
    {
        switch (migrationBuilder.ActiveProvider)
        {
            case ProviderNames.Sqlite: return; // does not support users
            /*
            case ProviderNames.PostgreSQL:
                {
                    migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA {DesicContext.RefSchema} REVOKE SELECT ON TABLES FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"REVOKE SELECT ON ALL TABLES IN SCHEMA {DesicContext.RefSchema} FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"REVOKE USAGE ON SCHEMA {DesicContext.RefSchema} FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA {DesicContext.AppSchema} REVOKE SELECT, INSERT, UPDATE, DELETE ON TABLES FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"REVOKE USAGE ON ALL SEQUENCES IN {DesicContext.AppSchema} FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"REVOKE SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA {DesicContext.AppSchema} FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"REVOKE USAGE ON SCHEMA {DesicContext.AppSchema} FROM {DesicContext.AppUser};");
                    migrationBuilder.Sql($"GRANT CREATE ON SCHEMA public TO PUBLIC;");
                    //migrationBuilder.Sql($"DROP OWNED BY {DesicContext.AppUserName};");
                    migrationBuilder.Sql($"DROP USER IF EXISTS {DesicContext.AppUser};");
                    return;
                }
            */
            case ProviderNames.SqlServer:
                {
                    migrationBuilder.Sql($"ALTER ROLE [{AppRole}] DROP MEMBER [{DesicContext.AppUser}];");
                    migrationBuilder.Sql($"DROP ROLE IF EXISTS [{AppRole}];");
                    migrationBuilder.Sql($"DROP USER IF EXISTS [{DesicContext.AppUser}];");
                    migrationBuilder.Sql($"DROP LOGIN [{AppUserLogin}];");
                    return;
                }
        }
        throw new Exception($"Provider is unsupported: {migrationBuilder.ActiveProvider}");
    }
}
