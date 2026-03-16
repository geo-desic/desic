namespace Desic.Infrastructure.Data;

public sealed class ApplicationDatabaseSeedingOptions
{
    public const string SectionName = "Databases:Application:Seeding";

    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingEntityTypesOptions? EntityTypes { get; set; }
    public ApplicationDatabaseSeedingIso3166CountriesOptions? Iso3166Countries { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
    public ApplicationDatabaseSeedingTagsOptions? Tags { get; set; }
    public ApplicationDatabaseSeedingTestOptions? Test { get; set; }
    public ApplicationDatabaseSeedingUsersOptions? Users { get; set; }
}

public sealed class ApplicationDatabaseSeedingEntityTypesOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDatabaseSeedingIso3166CountriesOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDatabaseSeedingUsersOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDatabaseSeedingTagsOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDatabaseSeedingTestOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
    public ApplicationDatabaseSeedingTestUsersOptions? Users { get; set; }
}

public sealed class ApplicationDatabaseSeedingTestUsersOptions
{
    public int? Count { get; set; }
    public bool? Enabled { get; set; }
    public ApplicationDatabaseSeedingMethod? Method { get; set; }
}

public enum ApplicationDatabaseSeedingMethod
{
    Fast,
    Full,
}