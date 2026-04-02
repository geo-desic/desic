using Desic.Application.Common.Interfaces;
using Desic.Domain.EntityTypes;
using Desic.Domain.Iso3166Countries;
using Desic.Domain.Labels;
using Desic.Domain.Processes;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Infrastructure.Data.Common.Extensions;
using Desic.Infrastructure.Data.Configurations;
using Desic.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Desic.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly bool _providerIsSqlServer;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _providerIsSqlServer = Database.IsSqlServer();
    }

    public Guid CreateSequentialGuid() => Guid.CreateSequentialGuid(forSqlServer: _providerIsSqlServer);

    // note: when adding a new DbSet<T> also add it to the IApplicationDbContext inside the Application project (if it needs to use it)
    // alphebetized dbsets
    public DbSet<EntityType> EntityTypes { get; set; }
    public DbSet<Iso3166Country> Iso3166Countries { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Process> Processes { get; set; }
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
        if (Database.SupportsSchemas()) modelBuilder.HasDefaultSchema(AppSchema);
        // alphabetized entity configurations
        modelBuilder.ApplyConfiguration(new EntityTypeConfiguration(Database));
        modelBuilder.ApplyConfiguration(new Iso3166CountryConfiguration(Database));
        modelBuilder.ApplyConfiguration(new LabelConfiguration(Database));
        modelBuilder.ApplyConfiguration(new ProcessConfiguration(Database));
        modelBuilder.ApplyConfiguration(new TagConfiguration(Database));
        modelBuilder.ApplyConfiguration(new UserConfiguration(Database));
        modelBuilder.SetUtcValueConverterForAllDateTimeProperties();
    }
}