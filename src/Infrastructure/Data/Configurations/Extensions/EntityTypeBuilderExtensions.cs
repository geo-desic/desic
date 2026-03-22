using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations.Extensions;

internal static class EntityTypeBuilderExtensions
{
    internal static int ConfigureBaseEntity<T>(this EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        var columnOrder = -1;
        builder.Property(x => x.Id).IsRequired().HasColumnOrder(++columnOrder);
        builder.HasKey(x => x.Id);
        return columnOrder;
    }

    internal static int ConfigureCreatableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : CreatableEntity
    {
        var columnOrder = builder.ConfigureBaseEntity();
        builder.Property(x => x.CreatedById).IsRequired().HasColumnOrder(++columnOrder);
        builder.Property(x => x.CreatedByName).IsRequired(false).HasMaxLength(CreatableEntity.MaxLengthCreatedByName).HasColumnOrder(++columnOrder);
        builder.Property(x => x.CreatedByTypeId).IsRequired().HasColumnOrder(++columnOrder);
        builder.Property(x => x.CreatedOn).IsRequired().HasDefaultValueSql(databaseFacade.DateTimeUtc()).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.CreatedById).IsUnique(false);
        builder.HasIndex(x => x.CreatedOn).IsUnique(false);
        builder.HasOne<EntityType>().WithMany().HasForeignKey(x => x.CreatedByTypeId).OnDelete(DeleteBehavior.NoAction).IsRequired();
        return columnOrder;
    }

    internal static int ConfigureModifiableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : ModifiableEntity
    {
        var columnOrder = builder.ConfigureCreatableEntity(databaseFacade);
        builder.Property(x => x.ModifiedById).IsRequired().HasColumnOrder(++columnOrder);
        builder.Property(x => x.ModifiedByName).IsRequired(false).HasMaxLength(ModifiableEntity.MaxLengthModifiedByName).HasColumnOrder(++columnOrder);
        builder.Property(x => x.ModifiedByTypeId).IsRequired().HasColumnOrder(++columnOrder);
        builder.Property(x => x.ModifiedOn).IsRequired().HasDefaultValueSql(databaseFacade.DateTimeUtc()).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.ModifiedById).IsUnique(false);
        builder.HasIndex(x => x.ModifiedOn).IsUnique(false);
        builder.HasOne<EntityType>().WithMany().HasForeignKey(x => x.ModifiedByTypeId).OnDelete(DeleteBehavior.NoAction).IsRequired();
        return columnOrder;
    }

    internal static int ConfigureSeedableSoftDeletableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : SeedableSoftDeletableEntity
    {
        var columnOrder = builder.ConfigureSoftDeletableEntity(databaseFacade);
        builder.Property(x => x.IsBeingSeeded).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.IsBeingSeeded).IsUnique(false);
        return columnOrder;
    }

    internal static int ConfigureSoftDeletableEntity<T>(this EntityTypeBuilder<T> builder, DatabaseFacade databaseFacade) where T : SoftDeletableEntity
    {
        var columnOrder = builder.ConfigureModifiableEntity(databaseFacade);
        builder.Property(x => x.DeletedById).HasColumnOrder(++columnOrder);
        builder.Property(x => x.DeletedByName).IsRequired(false).HasMaxLength(SoftDeletableEntity.MaxLengthDeletedByName).HasColumnOrder(++columnOrder);
        builder.Property(x => x.DeletedByTypeId).HasColumnOrder(++columnOrder);
        builder.Property(x => x.DeletedOn).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.DeletedById).IsUnique(false);
        builder.HasIndex(x => x.DeletedOn).IsUnique(false);
        builder.HasOne<EntityType>().WithMany().HasForeignKey(x => x.DeletedByTypeId).OnDelete(DeleteBehavior.NoAction);
        return columnOrder;
    }
}
