using Desic.Application.Common.Interfaces;
using Desic.Domain.EntityTypes;

namespace Desic.Application.Common.Extensions;

internal static class ModelExtensions
{
    public static void MapCreated(this ICreatable item, Domain.Common.Interfaces.IReadOnlyCreatable from)
    {
        var entityType = SystemEntityTypes.GetById(from.CreatedByTypeId);
        item.Created.By.Id = from.CreatedById;
        item.Created.By.Name = from.CreatedByName;
        item.Created.By.Type.Key = entityType?.Key ?? string.Empty;
        item.Created.By.Type.Name = entityType?.Name ?? string.Empty;
        item.Created.On = from.CreatedOn;
    }

    public static void MapCreatedModified<T, U>(this T item, U from)
        where T : ICreatable, IModifiable
        where U : Domain.Common.Interfaces.IReadOnlyCreatable, Domain.Common.Interfaces.IReadOnlyModifiable
    {
        item.MapCreated(from);
        item.MapModified(from);
    }

    public static void MapCreatedModifiedDeleted<T, U>(this T item, U from)
        where T : ICreatable, IModifiable, ISoftDeletable
        where U : Domain.Common.Interfaces.IReadOnlyCreatable, Domain.Common.Interfaces.IReadOnlyModifiable, Domain.Common.Interfaces.IReadOnlySoftDeletable
    {
        item.MapCreated(from);
        item.MapModified(from);
        item.MapDeleted(from);
    }

    public static void MapDeleted(this ISoftDeletable item, Domain.Common.Interfaces.IReadOnlySoftDeletable from)
    {
        var entityType = from.DeletedByTypeId.HasValue ? SystemEntityTypes.GetById(from.DeletedByTypeId.Value) : null;
        item.Deleted.By.Id = from.DeletedById;
        item.Deleted.By.Name = from.DeletedByName;
        item.Deleted.By.Type.Key = entityType?.Key;
        item.Deleted.By.Type.Name = entityType?.Name;
        item.Deleted.On = from.DeletedOn;
    }

    public static void MapId(this IGuidId item, Domain.Common.Interfaces.IReadOnlyGuidId from)
    {
        item.Id = from.Id;
    }

    public static void MapModified(this IModifiable item, Domain.Common.Interfaces.IReadOnlyModifiable from)
    {
        var entityType = SystemEntityTypes.GetById(from.ModifiedByTypeId);
        item.Modified.By.Id = from.ModifiedById;
        item.Modified.By.Name = from.ModifiedByName;
        item.Modified.By.Type.Key = entityType?.Key ?? string.Empty;
        item.Modified.By.Type.Name = entityType?.Name ?? string.Empty;
        item.Modified.On = from.ModifiedOn;
    }
}
