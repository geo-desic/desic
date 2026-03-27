using Desic.Domain.EntityTypes;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

internal class EntityTypeConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<EntityType>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<EntityType> builder)
    {
        var schema = _databaseFacade.IsSqlite() ? null : ApplicationDbContext.RefSchema;
        builder.ToTable(nameof(ApplicationDbContext.EntityTypes), schema);
        var columnOrder = builder.ConfigureBaseEntity();
        builder.Property(x => x.Key).IsRequired().HasMaxLength(EntityType.LengthKey).HasColumnOrder(++columnOrder);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(EntityType.MaxLengthName).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
