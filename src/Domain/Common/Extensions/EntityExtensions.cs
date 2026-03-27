using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Shared.Extensions;

namespace Desic.Domain.Common.Extensions;

public static class EntityExtensions
{
    public static void SetCreatedBy(this ICreatable entity, IReadOnlyMinimalEntity by, DateTime? on = null)
    {
        entity.CreatedById = by.Id;
        if (by is IReadOnlyNameable byNamed) entity.CreatedByName = byNamed.Name.Left(length: CreatableEntity.MaxLengthCreatedByName);
        entity.CreatedByTypeId = by.SystemEntityType.Id;
        entity.CreatedOn = on ?? DateTime.UtcNow;
    }

    public static void SetCreatedAndModifiedBy<T>(this T entity, IReadOnlyMinimalEntity by, DateTime? on = null) where T : ICreatable, IModifiable
    {
        on ??= DateTime.UtcNow;
        entity.SetCreatedBy(by, on: on);
        entity.SetModifiedBy(by, on: on);
    }

    public static void SetDeletedBy(this ISoftDeletable entity, IReadOnlyMinimalEntity by, DateTime? on = null)
    {
        entity.DeletedById = by.Id;
        if (by is IReadOnlyNameable byNamed) entity.DeletedByName = byNamed.Name.Left(length: SoftDeletableEntity.MaxLengthDeletedByName);
        entity.DeletedByTypeId = by.SystemEntityType.Id;
        entity.DeletedOn = on ?? DateTime.UtcNow;
    }

    public static void SetDeletedAndModifiedBy<T>(this T entity, IReadOnlyMinimalEntity by, DateTime? on = null) where T : IModifiable, ISoftDeletable
    {
        on ??= DateTime.UtcNow;
        entity.SetDeletedBy(by, on: on);
        entity.SetModifiedBy(by, on: on);
    }

    public static void SetModifiedBy(this IModifiable entity, IReadOnlyMinimalEntity by, DateTime? on = null)
    {
        entity.ModifiedById = by.Id;
        if (by is IReadOnlyNameable byNamed) entity.ModifiedByName = byNamed.Name.Left(length: ModifiableEntity.MaxLengthModifiedByName);
        entity.ModifiedByTypeId = by.SystemEntityType.Id;
        entity.ModifiedOn = on ?? DateTime.UtcNow;
    }

    public static void SetNotDeleted(this ISoftDeletable entity)
    {
        entity.DeletedById = null;
        entity.DeletedByName = null;
        entity.DeletedByTypeId = null;
        entity.DeletedOn = null;
    }

    public static void SetNotDeletedAndModifiedBy<T>(this T entity, IReadOnlyMinimalEntity by, DateTime? on = null) where T : IModifiable, ISoftDeletable
    {
        entity.SetNotDeleted();
        entity.SetModifiedBy(by, on);
    }
}
