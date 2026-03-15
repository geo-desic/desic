using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit;

public interface ITestDbContext : IBaseDbContext
{
    DbSet<TestEntity> TestEntities { get; }
    DbSet<IntIdEntity> IntIdEntities { get; }
}

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options), ITestDbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<IntIdEntity> IntIdEntities { get; set; }
}

public class TestEntity : BaseEntity
{
    public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
}

public class IntIdEntity // for tests that don't need an entity that inherits from BaseEntity
{
    public int Id { get; set; }
}