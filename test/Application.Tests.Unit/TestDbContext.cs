using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit;

internal interface ITestDbContext : IBaseDbContext
{
    DbSet<TestEntity> TestEntities { get; }
}

internal class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options), ITestDbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

internal class TestEntity : BaseEntity
{
    public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    public int SequentialId { get; set; }
}