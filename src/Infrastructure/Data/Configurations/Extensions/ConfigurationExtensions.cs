using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations.Extensions;

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
        builder.HasOne<EntityType>().WithMany().HasForeignKey(x => x.ModifiedByTypeId).OnDelete(DeleteBehavior.NoAction).IsRequired();
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
        builder.HasOne<EntityType>().WithMany().HasForeignKey(x => x.CreatedByTypeId).OnDelete(DeleteBehavior.NoAction).IsRequired();
        return columnOrder;
    }

    internal static int ConfigureMinimalEntity<T>(this EntityTypeBuilder<T> builder) where T : MinimalEntity
    {
        var columnOrder = 0;
        builder.Property(x => x.Id).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasKey(x => x.Id);
        return columnOrder;
    }

    internal static int ConfigureSoftDeletableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : SoftDeletableEntity
    {
        var columnOrder = builder.ConfigureModifiableEntity(databaseFacade);
        builder.Property(x => x.IsDeleted).HasColumnOrder(columnOrder++);
        builder.Property(x => x.DeletedById).HasColumnOrder(columnOrder++);
        builder.Property(x => x.DeletedByTypeId).HasColumnOrder(columnOrder++);
        builder.Property(x => x.DeletedOn).HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.IsDeleted).IsUnique(false);
        builder.HasIndex(x => x.DeletedById).IsUnique(false);
        builder.HasIndex(x => x.DeletedByTypeId).IsUnique(false);
        builder.HasOne<EntityType>().WithMany().HasForeignKey(x => x.DeletedByTypeId).OnDelete(DeleteBehavior.NoAction);
        return columnOrder;
    }

    internal static int ConfigureSeedableSoftDeletableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : SeedableSoftDeletableEntity
    {
        var columnOrder = builder.ConfigureSoftDeletableEntity(databaseFacade);
        builder.Property(x => x.IsBeingSeeded).HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.IsBeingSeeded).IsUnique(false);
        return columnOrder;
    }
}
