using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Application.Common.Helpers;

internal static class ByHelpers
{
    public static void MapCreated(this ICreated item, ICreatable entity)
    {
        var entityType = SystemEntityTypes.GetById(entity.CreatedByTypeId);
        item.Created.By.Id = entity.CreatedById;
        item.Created.By.Type.Key = entityType?.Key ?? string.Empty;
        item.Created.By.Type.Name = entityType?.Name ?? string.Empty;
        item.Created.On = entity.CreatedOn;
    }

    public static void MapModified(this IModified item, IModifiable entity)
    {
        var entityType = SystemEntityTypes.GetById(entity.ModifiedByTypeId);
        item.Modified.By.Id = entity.ModifiedById;
        item.Modified.By.Type.Key = entityType?.Key ?? string.Empty;
        item.Modified.By.Type.Name = entityType?.Name ?? string.Empty;
        item.Modified.On = entity.ModifiedOn;
    }

    public static void MapDeleted(this ISoftDeleted item, ISoftDeletable entity)
    {
        var entityType = entity.DeletedByTypeId.HasValue ? SystemEntityTypes.GetById(entity.DeletedByTypeId.Value) : null;
        item.Deleted.By.Id = entity.DeletedById;
        item.Deleted.By.Type.Key = entityType?.Key;
        item.Deleted.By.Type.Name = entityType?.Name;
        item.Deleted.On = entity.DeletedOn;
    }

    public static void MapCreatedModified<T, U>(this T item, U entity) where T : ICreated, IModified where U : ICreatable, IModifiable
    {
        item.MapCreated(entity);
        item.MapModified(entity);
    }

    public static void MapCreatedModifiedDeleted<T, U>(this T item, U entity) where T : ICreated, IModified, ISoftDeleted where U : ICreatable, IModifiable, ISoftDeletable
    {
        item.MapCreated(entity);
        item.MapModified(entity);
        item.MapDeleted(entity);
    }
}
