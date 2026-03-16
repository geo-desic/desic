using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Infrastructure.Data.Tags;

public class SeedTagsRequestHandlers(ApplicationDbContext context, ILogger<SeedTagsRequestHandlers> logger) : IRequestHandler<SeedTagsRequest, SeedTagsResult>
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SeedTagsRequestHandlers> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<SeedTagsResult> Handle(SeedTagsRequest request, CancellationToken cancellationToken)
    {
        var result = new SeedTagsResult();
        var dbSet = _context.Tags;
        var tableName = nameof(_context.Tags);

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && request?.Method != ApplicationDatabaseSeedingMethod.Full)
        {
            _logger.LogDebug("Skipping {TableName} as it already has records", tableName);
            return result;
        }

        var items = SystemTags.AllAsEntities().ToList();
        result.ReferenceCount = items.Count;
        if (!any) // fast method
        {
            await dbSet.AddRangeAsync(items, cancellationToken);
            result.Inserts = await _context.SaveChangesAsync(cancellationToken);
        }
        else // full method
        {
            var itemsToAdd = new List<Tag>();
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
                        existing.SetNotDeletedAndModifiedBy(request.By);
                        ++result.Updates;
                    }
                }
            }
            await dbSet.AddRangeAsync(itemsToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // no deletes
        }
        _context.ChangeTracker.Clear(); // no reason to track entities after the changes have been saved

        _logger.LogInformation("Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
        return result;
    }
}
