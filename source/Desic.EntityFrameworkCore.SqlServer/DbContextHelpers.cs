using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Desic.EntityFrameworkCore.SqlServer;

public static class DbContextHelpers
{
    public static async Task InitializeAsync(this DbContext context, string? targetDatabaseName = null, CancellationToken cancellationToken = default)
    {
        await context.GetService<DatabaseInitializer>().InitializeAsync((SqlConnection)context.Database.GetDbConnection(), targetDatabaseName: targetDatabaseName, cancellationToken: cancellationToken);
    }
}
