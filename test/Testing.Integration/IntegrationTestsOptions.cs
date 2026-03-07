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
    public IntegrationTestsDatabaseApplicationOptions? Application { get; set; }
}

public sealed class IntegrationTestsDatabaseApplicationOptions
{
    public IntegrationTestsDatabaseApplicationSqlServerOptions? SqlServer { get; set; }
}

public sealed class IntegrationTestsDatabaseApplicationSqlServerOptions
{
    public Dictionary<string, IntegrationTestsDatabaseApplicationSqlServerUsersOptions>? Users { get; set; }
}

public sealed class IntegrationTestsDatabaseApplicationSqlServerUsersOptions
{
    public string? Password { get; set; }
}