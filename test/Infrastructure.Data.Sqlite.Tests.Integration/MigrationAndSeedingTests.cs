using AwesomeAssertions;
using Desic.Testing.Integration;
using Desic.Testing.Integration.Db;
using Microsoft.EntityFrameworkCore;

namespace Desic.Infrastructure.Data.Sqlite.Tests.Integration;

public class MigrationAndSeedingTests
{
    [Fact]
    public async Task MigrateAsync_FromApplicationDbContextFactory_MigratesAndSeedsDatabase()
    {
        // arrange
        using var factory = new ApplicationDbContextFactory();
        await using var database = new EmptyDatabaseSqlite();
        await database.InitializeAsync();
        var context = factory.CreateDbContext(["--connection", database.GetConnectionString(), "--environment", Constants.TestEnvironmentName]);

        // act
        await context.Database.MigrateAsync(cancellationToken: TestContext.Current.CancellationToken);

        // assert
        (await context.EntityTypes.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeTrue();
        (await context.Iso3166Countries.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeTrue();
        (await context.Labels.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeTrue();
        (await context.Users.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeTrue();
    }

    [Fact]
    public async Task MigrateAsync_FromApplicationDbContextFactoryWithSeedingDisabled_MigratesAndDoesNotSeedDatabase()
    {
        // arrange
        using var factory = new ApplicationDbContextFactory();
        await using var database = new EmptyDatabaseSqlite();
        await database.InitializeAsync();
        var context = factory.CreateDbContext(["--connection", database.GetConnectionString(), "--environment", Constants.TestEnvironmentName, $"--{ApplicationDatabaseConfigKeys.SeedingEnabled}", $"{false}"]);

        // act
        await context.Database.MigrateAsync(cancellationToken: TestContext.Current.CancellationToken);

        // assert
        (await context.EntityTypes.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeFalse();
        (await context.Iso3166Countries.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeFalse();
        (await context.Labels.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeFalse();
        (await context.Users.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().BeFalse();
    }
}
