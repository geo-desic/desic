namespace Desic.Infrastructure.Data;

public sealed class DesicContextSeedingOptions
{
    public const string SectionName = "Databases:Desic:Seeding";

    public bool? Enabled { get; set; }
    public DesicContextSeedingEntityTypesOptions? EntityTypes { get; set; }
    public DesicContextSeedingIso3166CountriesOptions? Iso3166Countries { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
    public DesicContextSeedingTagsOptions? Tags { get; set; }
    public DesicContextSeedingTestOptions? Test { get; set; }
    public DesicContextSeedingUsersOptions? Users { get; set; }
}

public sealed class DesicContextSeedingEntityTypesOptions
{
    public bool? Enabled { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
}

public sealed class DesicContextSeedingIso3166CountriesOptions
{
    public bool? Enabled { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
}

public sealed class DesicContextSeedingUsersOptions
{
    public bool? Enabled { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
}

public sealed class DesicContextSeedingTagsOptions
{
    public bool? Enabled { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
}

public sealed class DesicContextSeedingTestOptions
{
    public bool? Enabled { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
    public DesicContextSeedingTestUsersOptions? Users { get; set; }
}

public sealed class DesicContextSeedingTestUsersOptions
{
    public int? Count { get; set; }
    public bool? Enabled { get; set; }
    public DesicContextSeedingMethod? Method { get; set; }
}

public enum DesicContextSeedingMethod
{
    Fast,
    Full,
}