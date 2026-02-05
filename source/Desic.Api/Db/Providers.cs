using Microsoft.EntityFrameworkCore;

namespace Desic.Api.Db
{
    public static class Providers
    {
        public static readonly Provider Sqlite = new (nameof(Sqlite), typeof(EntityFrameworkCore.Sqlite.Marker).Assembly.GetName().Name!);
        public static readonly Provider SqlServer = new (nameof(SqlServer), typeof(EntityFrameworkCore.SqlServer.Marker).Assembly.GetName().Name!);

        public static void Configure(IConfiguration config, DbContextOptionsBuilder options, string dbProvider)
        {
            if (dbProvider == Sqlite.Name)
            {
                options.UseSqlite(config.GetConnectionString(Sqlite.Name), x => x.MigrationsAssembly(Sqlite.Assembly));
            }
            else if (dbProvider == SqlServer.Name)
            {
                options.UseSqlServer(config.GetConnectionString(SqlServer.Name), x => x.MigrationsAssembly(SqlServer.Assembly));
            }
        }
    }
}
