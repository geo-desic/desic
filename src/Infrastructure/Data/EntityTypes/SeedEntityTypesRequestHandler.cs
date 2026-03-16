using Desic.Domain.EntityTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Infrastructure.Data.EntityTypes;

public class SeedEntityTypesRequestHandler(ApplicationDbContext context, ILogger<SeedEntityTypesRequestHandler> logger) : IRequestHandler<SeedEntityTypesRequest, SeedEntityTypesResult>
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SeedEntityTypesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<SeedEntityTypesResult> Handle(SeedEntityTypesRequest request, CancellationToken cancellationToken)
    {
        var result = new SeedEntityTypesResult();
        var dbSet = _context.EntityTypes;
        var tableName = nameof(_context.EntityTypes);

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && request.Method != ApplicationDatabaseSeedingMethod.Full)
        {
            _logger.LogDebug("Skipping {TableName} as it already has records", tableName);
            return result;
        }

        var items = SystemEntityTypes.AllAsEntities().ToList();
        result.ReferenceCount = items.Count;
        if (!any) // fast method
        {
            await dbSet.AddRangeAsync(items, cancellationToken);
            result.Inserts = await _context.SaveChangesAsync(cancellationToken);
        }
        else // full method
        {
            var itemsToAdd = new List<EntityType>();
            foreach (var item in items)
            {
                var existing = await dbSet.AsTracking().FirstOrDefaultAsync(x => x.Id == item.Id, cancellationToken);
                if (existing == null) // inserts
                {
                    itemsToAdd.Add(item);
                    ++result.Inserts;
                }
                else // updates
                {
                    if (existing.Name != item.Name) // so result.Updates will accurately reflect if an update occurred
                    {
                        existing.Name = item.Name;
                        ++result.Updates;
                    }
                }
            }
            await dbSet.AddRangeAsync(itemsToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // deletes
            var ids = items.Select(x => x.Id).ToList();
            // note: this bulk delete method will likely not be appropriate for other entity types
            // this is safe because the set of all entity types is small and completely defined by the Desic.Domain assembly; for reference see Desic.Domain.EntityTypes.SystemEntityType and SystemEntityTypes.Generate()
            // therefore database inserts/updates/deletes (other that this seeding code) should never occur and if it does it is safe to overwrite/delete it (e.g. corrupted database table data)
            result.Deletes = await dbSet.Where(e => !ids.Contains(e.Id)).ExecuteDeleteAsync(cancellationToken);
        }
        _context.ChangeTracker.Clear(); // no reason to track entities after the changes have been saved

        _logger.LogInformation("Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
        return result;
    }
}
