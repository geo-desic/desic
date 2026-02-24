using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Desic.Infrastructure.SqlServer;

public static class DbContextHelpers
{
    public static async Task InitializeAsync(this DbContext context, string? targetDatabaseName = null, CancellationToken cancellationToken = default)
    {
        var connectionString = context.Database.GetConnectionString() ?? throw new InvalidOperationException("Connection string could not be determined from the DbContext");
        await context.GetService<DatabaseInitializer>().InitializeAsync(connectionString: connectionString, targetDatabaseName: targetDatabaseName, cancellationToken: cancellationToken);
    }
}
