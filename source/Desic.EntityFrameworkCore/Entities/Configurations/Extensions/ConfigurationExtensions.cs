using Desic.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static int ConfigureModifiableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade db) where T : ModifiableEntity
        {
            var columnOrder = builder.ConfigureCreatableEntity(db);
            builder.Property(x => x.ModifiedById).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.ModifiedByTypeId).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.ModifiedOn).HasDefaultValueSql(db.DateTimeUtc()).IsRequired().HasColumnOrder(columnOrder++);
            builder.HasIndex(x => x.ModifiedById);
            builder.HasIndex(x => x.ModifiedByTypeId);
            return columnOrder;
        }

        internal static int ConfigureCreatableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade db) where T : CreatableEntity
        {
            var columnOrder = builder.ConfigureMinimalEntity();
            builder.Property(x => x.CreatedById).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.CreatedByTypeId).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.CreatedOn).HasDefaultValueSql(db.DateTimeUtc()).IsRequired().HasColumnOrder(columnOrder++);
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.CreatedByTypeId);
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
