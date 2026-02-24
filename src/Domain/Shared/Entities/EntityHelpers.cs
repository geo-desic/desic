namespace Desic.Domain.Shared.Entities;

public static class EntityHelpers
{
    public static void SetCreatedBy(this ICreatable entity, IReadOnlyMinimalEntity by, DateTime? on = null)
    {
        entity.CreatedById = by.Id;
        entity.CreatedByTypeId = by.GetEntityType().Id;
        entity.CreatedOn = on ?? DateTime.UtcNow;
    }

    public static void SetModifiedBy(this IModifiable entity, IReadOnlyMinimalEntity by, DateTime? on = null)
    {
        entity.ModifiedById = by.Id;
        entity.ModifiedByTypeId = by.GetEntityType().Id;
        entity.ModifiedOn = on ?? DateTime.UtcNow;
    }

    public static void SetCreatedAndModifiedBy<T>(this T entity, IReadOnlyMinimalEntity by, DateTime? on = null) where T : ICreatable, IModifiable
    {
        on ??= DateTime.UtcNow;
        entity.SetCreatedBy(by, on: on);
        entity.SetModifiedBy(by, on: on);
    }

    public static void SetDeletedBy(this ISoftDeletable entity, IReadOnlyMinimalEntity by, DateTime? on = null, bool setIsDeleted = true)
    {
        if (setIsDeleted) entity.IsDeleted = true;
        entity.DeletedById = by.Id;
        entity.DeletedByTypeId = by.GetEntityType().Id;
        entity.DeletedOn = on ?? DateTime.UtcNow;
    }

    public static void SetDeletedAndModifiedBy<T>(this T entity, IReadOnlyMinimalEntity by, DateTime? on = null, bool setIsDeleted = true) where T : IModifiable, ISoftDeletable
    {
        on ??= DateTime.UtcNow;
        entity.SetDeletedBy(by, on: on, setIsDeleted: setIsDeleted);
        entity.SetModifiedBy(by, on: on);
    }

    public static void SetNotDeleted(this ISoftDeletable entity)
    {
        entity.IsDeleted = false;
        entity.DeletedById = null;
        entity.DeletedByTypeId = null;
        entity.DeletedOn = null;
    }

    public static void SetNotDeletedAndModifiedBy<T>(this T entity, IReadOnlyMinimalEntity by, DateTime? on = null) where T : IModifiable, ISoftDeletable
    {
        entity.SetNotDeleted();
        entity.SetModifiedBy(by, on);
    }
}
