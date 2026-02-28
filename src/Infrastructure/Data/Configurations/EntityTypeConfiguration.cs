using Desic.Domain.EntityTypes;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

internal class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
{
    public void Configure(EntityTypeBuilder<EntityType> builder)
    {
        builder.ToTable("EntityTypes", ApplicationDbContext.RefSchema);
        var columnOrder = builder.ConfigureBaseEntity();
        builder.Property(x => x.Key).IsRequired().HasColumnOrder(columnOrder++);
        builder.Property(x => x.Name).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
