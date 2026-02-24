using Desic.Domain.EntityTypes;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

internal class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
{
    public void Configure(EntityTypeBuilder<EntityType> builder)
    {
        builder.ToTable("EntityTypes", DesicContext.RefSchema);
        var columnOrder = builder.ConfigureMinimalEntity();
        builder.Property(x => x.Name).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
