using Desic.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static int ConfigureModifiableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : ModifiableEntity
        {
            var columnOrder = builder.ConfigureCreatableEntity(databaseFacade);
            builder.Property(x => x.ModifiedById).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.ModifiedByTypeId).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.ModifiedOn).IsRequired().HasDefaultValueSql(databaseFacade.DateTimeUtc()).HasColumnOrder(columnOrder++);
            builder.HasIndex(x => x.ModifiedById).IsUnique(false);
            builder.HasIndex(x => x.ModifiedByTypeId).IsUnique(false);
            builder.HasOne<EntityType>().WithOne().HasForeignKey<T>(x => x.ModifiedByTypeId).OnDelete(DeleteBehavior.Restrict).IsRequired();
            return columnOrder;
        }

        internal static int ConfigureCreatableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : CreatableEntity
        {
            var columnOrder = builder.ConfigureMinimalEntity();
            builder.Property(x => x.CreatedById).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.CreatedByTypeId).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.CreatedOn).IsRequired().HasDefaultValueSql(databaseFacade.DateTimeUtc()).HasColumnOrder(columnOrder++);
            builder.HasIndex(x => x.CreatedById).IsUnique(false);
            builder.HasIndex(x => x.CreatedByTypeId).IsUnique(false);
            builder.HasOne<EntityType>().WithOne().HasForeignKey<T>(x => x.CreatedByTypeId).OnDelete(DeleteBehavior.Restrict).IsRequired();
            return columnOrder;
        }

        internal static int ConfigureMinimalEntity<T>(this EntityTypeBuilder<T> builder) where T : MinimalEntity
        {
            var columnOrder = 0;
            builder.Property(x => x.Id).IsRequired().HasColumnOrder(columnOrder++);
            builder.HasKey(x => x.Id);
            return columnOrder;
        }
    }
}
