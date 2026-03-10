using Desic.Infrastructure.Data.SqlServer;

namespace Desic.Testing.Integration;

public sealed class IntegrationTestsOptions
{
    public IntegrationTestsConnectionStringsOptions? ConnectionStrings { get; init; }
    public IntegrationTestsDbProvidersOptions? DbProviders { get; init; }
    public IntegrationTestsDatabaseOptions? Databases { get; init; }
    public DbProvider DbProvider { get; init; } = DbProvider.SqlServer;
}

public sealed class IntegrationTestsConnectionStringsOptions
{
    public string? SqlServer { get; init; }
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
    public DatabaseInitializerOptions? SqlServer { get; init; }
}

public enum DbProvider
{
    Sqlite,
    SqlServer,
}