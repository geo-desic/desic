using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit;

internal class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

internal class TestEntity
{
    public int Id { get; set; }
}