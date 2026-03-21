namespace Desic.Infrastructure.Data;

public sealed class SeedApplicationDatabaseOptions
{
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseEntityTypesOptions? EntityTypes { get; set; }
    public SeedApplicationDatabaseIso3166CountriesOptions? Iso3166Countries { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
    public SeedApplicationDatabaseLabelsOptions? Labels { get; set; }
    public SeedApplicationDatabaseTestOptions? Test { get; set; }
    public SeedApplicationDatabaseTestUsersOptions? Users { get; set; }
}

public sealed class SeedApplicationDatabaseEntityTypesOptions
{
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
}

public sealed class SeedApplicationDatabaseIso3166CountriesOptions
{
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
}

public sealed class SeedApplicationDatabaseTestUsersOptions
{
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
}

public sealed class SeedApplicationDatabaseLabelsOptions
{
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
}

public sealed class SeedApplicationDatabaseTestOptions
{
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
    public ApplicationDatabaseSeedingTestUsersOptions? Users { get; set; }
}

public sealed class ApplicationDatabaseSeedingTestUsersOptions
{
    public int? Count { get; set; }
    public bool? Enabled { get; set; }
    public SeedApplicationDatabaseMethod? Method { get; set; }
}

public enum SeedApplicationDatabaseMethod
{
    Fast,
    Full,
}