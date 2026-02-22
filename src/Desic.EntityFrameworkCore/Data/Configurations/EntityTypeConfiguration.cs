using Desic.Data.EntityTypes;
using Desic.EntityFrameworkCore.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Data.Configurations;

internal class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
{
    public void Configure(EntityTypeBuilder<EntityType> builder)
    {
        var columnOrder = builder.ConfigureMinimalEntity();
        builder.Property(x => x.Name).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
