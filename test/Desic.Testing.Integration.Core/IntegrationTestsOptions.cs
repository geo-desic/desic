namespace Desic.Testing.Integration.Core;

public sealed class IntegrationTestsOptions
{
    public IntegrationTestsContainersOptions? Containers { get; set; }
    public IntegrationTestsDatabaseOptions? Databases { get; set; }
    public string? DbProvider { get; set; }
}

public sealed class IntegrationTestsContainersOptions
{
    public DesicContextContainersMsSqlOptions? MsSql { get; set; }
}

public sealed class DesicContextContainersMsSqlOptions
{
    public string? Image { get; set; }
}

public sealed class IntegrationTestsDatabaseOptions
{
    public IntegrationTestsDatabaseDesicOptions? Desic { get; set; }
}

public sealed class IntegrationTestsDatabaseDesicOptions
{
    public string? AppUserPassword { get; set; }
    public bool? UseContainer { get; set; }
}
