using Desic.Application.Common;
using Desic.Domain.Common.Extensions;
using Desic.Domain.Iso3166Countries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Infrastructure.Data.Iso3166Countries;

public class SeedIso3166CountriesRequestHandler(ApplicationDbContext context, ILogger<SeedIso3166CountriesRequestHandler> logger, IMediator mediator) : IRequestHandler<SeedIso3166CountriesRequest, SeedIso3166CountriesResult>
{
    private int _batchNumber = 0;
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SeedIso3166CountriesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    private const int DefaultBatchSize = 50;
    private const int LogEventId = LogEvents.SeedIso3166Countries;

    public async Task<SeedIso3166CountriesResult> Handle(SeedIso3166CountriesRequest request, CancellationToken cancellationToken)
    {
        var result = new SeedIso3166CountriesResult();
        _batchNumber = 0;
        request.BatchSize ??= DefaultBatchSize;

        _context.ChangeTracker.Clear(); // the ChangeTracker does not work properly with the dbSet.ExecuteUpdateAsync calls below, so clear to make sure no existing tracked entities cause issues
        var dbSet = _context.Iso3166Countries;
        var tableName = nameof(_context.Iso3166Countries);
        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && request.Method != SeedApplicationDatabaseMethod.Full)
        {
            _logger.LogDebug(LogEventId, "Skipping {TableName} as it already has records", tableName);
            return result;
        }

        _logger.LogDebug(LogEventId, "Updating all records with IsBeingSeeded = true");
        await dbSet.ExecuteUpdateAsync(c => c.SetProperty(p => p.IsBeingSeeded, p => true), cancellationToken);

        var requestStream = new Iso3166CountriesResourceStreamRequest
        {
            Assembly = typeof(IAssemblyReference).Assembly,
            ClassMapType = typeof(Iso3166CountryClassMap),
            ResourceName = $"{typeof(Iso3166CountryClassMap).Namespace}.iso-3166-countries.csv",
        };

        var batchInserts = new List<Iso3166Country>();
        _logger.LogDebug(LogEventId, "Starting comparison of data to determine if any records need to be inserted or updated");
        _logger.LogDebug(LogEventId, "Creating stream for csv resource = {ResourceName} using class map type = {ClassMapType}", requestStream.ResourceName, requestStream.ClassMapType);
        await foreach (var item in _mediator.CreateStream(requestStream, cancellationToken))
        {
            ++result.ReferenceCount;
            var countryExisting = await dbSet.AsTracking().FirstOrDefaultAsync(x => x.IsoId == item.IsoId, cancellationToken);
            if (countryExisting == null)
            {
                item.Id = _context.CreateSequentialGuid();
                item.IsBeingSeeded = false;
                item.SetCreatedAndModifiedBy(request.By);
                batchInserts.Add(item);
                ++result.Inserts;
                // will be persisted on next PerformBatchChanges
            }
            else
            {
                countryExisting.IsBeingSeeded = false;
                if (!countryExisting.IsEquivalentTo(item))
                {
                    countryExisting.UpdateFrom(item);
                    countryExisting.SetNotDeletedAndModifiedBy(request.By);
                    ++result.Updates;
                    // will be persisted on next PerformBatchChanges
                }
            }
            if (result.ReferenceCount % request.BatchSize == 0)
            {
                // no reason to track any inserted/updated items once they are saved to the context
                await PerformBatchChanges(cancellationToken: cancellationToken, batchInserts: batchInserts, clearChangeTracker: true);
            }
        }
        if (result.ReferenceCount % request.BatchSize != 0) // for potential partial batch at end
        {
            await PerformBatchChanges(cancellationToken: cancellationToken, batchInserts: batchInserts, clearChangeTracker: true);
        }

        // soft deletes (any records with IsBeingSeeded == true since this was set to false for all inserts/updates above)
        _logger.LogDebug(LogEventId, "Determining if any records need to be (soft) deleted");
        var now = DateTime.UtcNow;
        result.Deletes = await dbSet
            .Where(c => c.IsBeingSeeded)
            .ExecuteUpdateAsync(c => c
                .SetProperty(p => p.DeletedById, p => request.By.Id)
                .SetProperty(p => p.DeletedByTypeId, p => request.By.SystemEntityType.Id)
                .SetProperty(p => p.DeletedOn, p => now)
                .SetProperty(p => p.ModifiedById, p => request.By.Id)
                .SetProperty(p => p.ModifiedByTypeId, p => request.By.SystemEntityType.Id)
                .SetProperty(p => p.ModifiedOn, p => now), cancellationToken);

        _logger.LogInformation(LogEventId, "Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
        return result;
    }

    private async Task PerformBatchChanges(CancellationToken cancellationToken, List<Iso3166Country>? batchInserts = null, bool clearChangeTracker = false)
    {
        ++_batchNumber;
        if (batchInserts != null)
        {
            await _context.Iso3166Countries.AddRangeAsync(batchInserts, cancellationToken);
        }
        _logger.LogDebug(LogEventId, "Saving changes for batch {BatchNumber}", _batchNumber);
        await _context.SaveChangesAsync(cancellationToken);
        batchInserts?.Clear();
        if (clearChangeTracker)
        {
            _context.ChangeTracker.Clear();
        }
    }
}
