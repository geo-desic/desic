namespace Desic.Testing.Integration;

public sealed class IntegrationTestsOptions
{
    public IntegrationTestsDbProvidersOptions? DbProviders { get; set; }
    public IntegrationTestsDatabaseOptions? Databases { get; set; }
    public string? DbProvider { get; set; }
}

public sealed class IntegrationTestsDbProvidersOptions
{
    public IntegrationTestsDbProvidersSqlServerOptions? SqlServer { get; set; }
}

public sealed class IntegrationTestsDbProvidersSqlServerOptions
{
    public bool? UseContainer { get; set; }
    public string? ContainerImage { get; set; }
}

public sealed class IntegrationTestsDatabaseOptions
{
    public IntegrationTestsDatabaseDesicOptions? Desic { get; set; }
}

public sealed class IntegrationTestsDatabaseDesicOptions
{
    public string? ApiUserPassword { get; set; }
}
