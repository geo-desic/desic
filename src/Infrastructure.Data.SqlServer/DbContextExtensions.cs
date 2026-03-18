using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Desic.Infrastructure.Data.SqlServer;

public static class DbContextExtensions
{
    public static async Task InitializeAsync(this DbContext context, string? targetDatabaseName = null, CancellationToken cancellationToken = default)
    {
        var connectionString = context.Database.GetConnectionString() ?? throw new InvalidOperationException("Connection string could not be determined from the DbContext");
        await context.GetService<InitializeApplicationDatabaseRequestHandler>().Handle(new InitializeApplicationDatabaseRequest { ConnectionString = connectionString, DatabaseName = targetDatabaseName }, cancellationToken: cancellationToken);
    }
}
