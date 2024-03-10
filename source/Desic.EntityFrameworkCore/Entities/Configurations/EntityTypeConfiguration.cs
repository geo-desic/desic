using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations
{
    internal class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
    {
        public void Configure(EntityTypeBuilder<EntityType> builder)
        {
            builder.HasKey(x => x.Id);
            //builder.HasIndex(x => x.SequentialId, "UX_EntityTypes_SequentialId").IsUnique();
            builder.HasIndex(x => x.Name, "UX_EntityTypes_Username").IsUnique();
        }
    }
}
