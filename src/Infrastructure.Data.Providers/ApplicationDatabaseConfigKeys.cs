namespace Desic.Infrastructure.Data.Providers;

public static class ApplicationDatabaseConfigKeys
{
    public static class SqlServer
    {
        public const string Section = "Databases:Application:SqlServer";
        public const string SectionApi = $"{Section}:Api";
        public const string SectionApiConnectionBehavior = $"{SectionApi}:ConnectionBehavior";
        public const string SectionInitialization = $"{Section}:Initialization";
        public const string SectionInitializationConnectionBehavior = $"{SectionInitialization}:ConnectionBehavior";
        public const string SectionMigrations = $"{Section}:Migrations";
        public const string SectionMigrationsConnectionBehavior = $"{SectionMigrations}:ConnectionBehavior";
    }
}
