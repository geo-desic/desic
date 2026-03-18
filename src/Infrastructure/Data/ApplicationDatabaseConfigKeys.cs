namespace Desic.Infrastructure.Data;

public static class ApplicationDatabaseConfigKeys
{
    // sections
    public const string Section = "Databases:Application";
    public const string SectionMigrations = $"{Section}:Migrations";
    public const string SectionSeeding = $"{Section}:Seeding";

    public const string MigrationsEnabled = $"{SectionMigrations}:Enabled";
    public const string SeedingEnabled = $"{SectionSeeding}:Enabled";
}
