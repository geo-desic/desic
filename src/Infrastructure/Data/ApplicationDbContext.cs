using Desic.Application.Common.Interfaces;
using Desic.Domain.EntityTypes;
using Desic.Domain.Iso3166Countries;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Desic.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    // note: when adding a new DbSet<T> also add it to the IApplicationDbContext inside the Application project (if it needs to use it)
    public DbSet<EntityType> EntityTypes { get; set; }
    public DbSet<Iso3166Country> Iso3166Countries { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }

    public const string AppSchema = "app"; // application can perform all dml operations (select, insert, update, delete) in this schema
    public const string RefSchema = "ref"; // application can perform only read operations (select) in this schema

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // queries should use AsTracking() when necessary
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(AppSchema);
        modelBuilder.ApplyConfiguration(new EntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new Iso3166CountryConfiguration(Database));
        modelBuilder.ApplyConfiguration(new TagConfiguration(Database));
        modelBuilder.ApplyConfiguration(new UserConfiguration(Database));
    }
}