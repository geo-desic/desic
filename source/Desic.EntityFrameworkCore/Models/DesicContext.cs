using Desic.EntityFrameworkCore.Data;
using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Entities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Desic.EntityFrameworkCore.Models;

public class DesicContext(DbContextOptions<DesicContext> options) : DbContext(options)
{
    public DbSet<EntityType> EntityTypes { get; set; }
    public DbSet<Iso3166Country> Iso3166Countries { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("app");
        modelBuilder.ApplyConfiguration(new EntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new Iso3166CountryConfiguration(Database));
        modelBuilder.ApplyConfiguration(new TagConfiguration(Database));
        modelBuilder.ApplyConfiguration(new UserConfiguration(Database));
    }

    public static async Task InitializeAsync(DesicContext db, CancellationToken cancellationToken)
    {
        await db.Database.MigrateAsync(cancellationToken);
        await Seed.ApplyAsync(db, cancellationToken);
    }
}