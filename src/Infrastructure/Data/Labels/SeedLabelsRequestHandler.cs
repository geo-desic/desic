using Desic.Application.Common;
using Desic.Domain.Common.Entities;
using Desic.Domain.Labels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Infrastructure.Data.Labels;

public class SeedLabelsRequestHandler(ApplicationDbContext context, ILogger<SeedLabelsRequestHandler> logger) : IRequestHandler<SeedLabelsRequest, SeedLabelsResult>
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SeedLabelsRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const int LogEventId = LogEvents.SeedLabels;

    public async Task<SeedLabelsResult> Handle(SeedLabelsRequest request, CancellationToken cancellationToken)
    {
        var result = new SeedLabelsResult();
        var dbSet = _context.Labels;
        var tableName = nameof(_context.Labels);

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && request?.Method != SeedApplicationDatabaseMethod.Full)
        {
            _logger.LogDebug(LogEventId, "Skipping {TableName} as it already has records", tableName);
            return result;
        }

        var items = SystemLabels.AllAsEntities().ToList();
        result.ReferenceCount = items.Count;
        if (!any) // fast method
        {
            await dbSet.AddRangeAsync(items, cancellationToken);
            result.Inserts = await _context.SaveChangesAsync(cancellationToken);
        }
        else // full method
        {
            var itemsToAdd = new List<Label>();
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

        _logger.LogInformation(LogEventId, "Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
        return result;
    }
}
