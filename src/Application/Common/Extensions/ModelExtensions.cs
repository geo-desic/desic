using Desic.Application.Common.Interfaces;
using Desic.Domain.EntityTypes;

namespace Desic.Application.Common.Extensions;

internal static class ModelExtensions
{
    public static void MapCreated(this ICreatable item, Domain.Common.Interfaces.IReadOnlyCreatable entity)
    {
        var entityType = SystemEntityTypes.GetById(entity.CreatedByTypeId);
        item.Created.By.Id = entity.CreatedById;
        item.Created.By.Name = entity.CreatedByName;
        item.Created.By.Type.Key = entityType?.Key ?? string.Empty;
        item.Created.By.Type.Name = entityType?.Name ?? string.Empty;
        item.Created.On = entity.CreatedOn;
    }

    public static void MapCreatedModified<T, U>(this T item, U entity)
        where T : ICreatable, IModifiable
        where U : Domain.Common.Interfaces.IReadOnlyCreatable, Domain.Common.Interfaces.IReadOnlyModifiable
    {
        item.MapCreated(entity);
        item.MapModified(entity);
    }

    public static void MapCreatedModifiedDeleted<T, U>(this T item, U entity)
        where T : ICreatable, IModifiable, ISoftDeletable
        where U : Domain.Common.Interfaces.IReadOnlyCreatable, Domain.Common.Interfaces.IReadOnlyModifiable, Domain.Common.Interfaces.IReadOnlySoftDeletable
    {
        item.MapCreated(entity);
        item.MapModified(entity);
        item.MapDeleted(entity);
    }

    public static void MapDeleted(this ISoftDeletable item, Domain.Common.Interfaces.IReadOnlySoftDeletable entity)
    {
        var entityType = entity.DeletedByTypeId.HasValue ? SystemEntityTypes.GetById(entity.DeletedByTypeId.Value) : null;
        item.Deleted.By.Id = entity.DeletedById;
        item.Deleted.By.Name = entity.DeletedByName;
        item.Deleted.By.Type.Key = entityType?.Key;
        item.Deleted.By.Type.Name = entityType?.Name;
        item.Deleted.On = entity.DeletedOn;
    }

    public static void MapId(this IGuidId item, Domain.Common.Interfaces.IReadOnlyId entity)
    {
        item.Id = entity.Id;
    }

    public static void MapModified(this IModifiable item, Domain.Common.Interfaces.IReadOnlyModifiable entity)
    {
        var entityType = SystemEntityTypes.GetById(entity.ModifiedByTypeId);
        item.Modified.By.Id = entity.ModifiedById;
        item.Modified.By.Name = entity.ModifiedByName;
        item.Modified.By.Type.Key = entityType?.Key ?? string.Empty;
        item.Modified.By.Type.Name = entityType?.Name ?? string.Empty;
        item.Modified.On = entity.ModifiedOn;
    }
}
