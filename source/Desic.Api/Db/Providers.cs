using Microsoft.EntityFrameworkCore;

namespace Desic.Api.Db;

public static class Providers
{
    public const string Sqlite = "Sqlite";
    public const string SqlServer = "SqlServer";

    public static void UseProvider(this DbContextOptionsBuilder options, string dbProvider, IConfiguration config)
    {
        _ = dbProvider switch
        {
            Sqlite => options.UseSqlite(config.GetConnectionString(Sqlite)),
            SqlServer => options.UseSqlServer(config.GetConnectionString(SqlServer)),
            _ => throw new Exception($"Unsupported db provider: {dbProvider}"),
        };
    }
}
