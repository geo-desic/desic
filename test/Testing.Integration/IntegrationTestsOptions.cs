namespace Desic.Testing.Integration;

public sealed class IntegrationTestsOptions
{
    public IntegrationTestsDbProvidersOptions? DbProviders { get; init; }
    public IntegrationTestsDatabaseOptions? Databases { get; init; }
    public string? DbProvider { get; init; }
}

public sealed class IntegrationTestsDbProvidersOptions
{
    public IntegrationTestsDbProvidersSqlServerOptions? SqlServer { get; init; }
}

public sealed class IntegrationTestsDbProvidersSqlServerOptions
{
    public bool? UseContainer { get; init; }
    public string? ContainerImage { get; init; }
}

public sealed class IntegrationTestsDatabaseOptions
{
    public IntegrationTestsDatabaseApplicationOptions? Application { get; init; }
}

public sealed class IntegrationTestsDatabaseApplicationOptions
{
    public IntegrationTestsDatabaseApplicationSqlServerOptions? SqlServer { get; init; }
}

public sealed class IntegrationTestsDatabaseApplicationSqlServerOptions
{
    public Dictionary<string, IntegrationTestsDatabaseApplicationSqlServerUsersOptions>? Users { get; init; }
}

public sealed class IntegrationTestsDatabaseApplicationSqlServerUsersOptions
{
    public string? Password { get; init; }
}