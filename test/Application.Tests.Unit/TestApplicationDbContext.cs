using Desic.Application.Common.Interfaces;
using Desic.Domain.EntityTypes;
using Desic.Domain.Iso3166Countries;
using Desic.Domain.Labels;
using Desic.Domain.Processes;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit;

public class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly bool _isSqlServer;

    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options) : base(options)
    {
        _isSqlServer = Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer";
    }

    public Guid CreateSequentialGuid() => Guid.CreateSequentialGuid(forSqlServer: _isSqlServer);

    // alphebetized dbsets
    public DbSet<EntityType> EntityTypes { get; set; }
    public DbSet<Iso3166Country> Iso3166Countries { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Process> Processes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }
}
