using Microsoft.Data.SqlClient;

namespace Desic.Infrastructure.SqlServer;

public static class SqlConnectionHelpers
{
    public static string? GetDatabaseFromConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        return builder.InitialCatalog == string.Empty ? null : builder.InitialCatalog;
    }

    public static string? UpdateDatabaseInConnectionString(string connectionString, string database)
    {
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = database };
        return builder.ConnectionString;
    }
}
