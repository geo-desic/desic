namespace Desic.Infrastructure.Data;

public sealed class ApplicationDbContextSeedingOptions
{
    public const string SectionName = "Databases:Desic:Seeding";

    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingEntityTypesOptions? EntityTypes { get; set; }
    public ApplicationDbContextSeedingIso3166CountriesOptions? Iso3166Countries { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
    public ApplicationDbContextSeedingTagsOptions? Tags { get; set; }
    public ApplicationDbContextSeedingTestOptions? Test { get; set; }
    public ApplicationDbContextSeedingUsersOptions? Users { get; set; }
}

public sealed class ApplicationDbContextSeedingEntityTypesOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDbContextSeedingIso3166CountriesOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDbContextSeedingUsersOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDbContextSeedingTagsOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
}

public sealed class ApplicationDbContextSeedingTestOptions
{
    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
    public ApplicationDbContextSeedingTestUsersOptions? Users { get; set; }
}

public sealed class ApplicationDbContextSeedingTestUsersOptions
{
    public int? Count { get; set; }
    public bool? Enabled { get; set; }
    public ApplicationDbContextSeedingMethod? Method { get; set; }
}

public enum ApplicationDbContextSeedingMethod
{
    Fast,
    Full,
}