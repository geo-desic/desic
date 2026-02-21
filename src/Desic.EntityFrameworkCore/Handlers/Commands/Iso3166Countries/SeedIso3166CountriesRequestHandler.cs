using Desic.Data.Entities;
using Desic.Data.Entities.Infrastructure;
using Desic.Data.Enums;
using Desic.Data.Models;
using Desic.Data.Requests.Commands.Iso3166Countries;
using Desic.Data.Requests.Queries.Resources;
using Desic.Data.Resources.ClassMaps;
using Desic.EntityFrameworkCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Handlers.Commands.Iso3166Countries;

internal class SeedIso3166CountriesRequestHandler(DesicContext context, ILogger<SeedIso3166CountriesRequestHandler> logger, IMediator mediator) : IRequestHandler<SeedIso3166CountriesRequest, EntitySetSeedingResult>
{
    private int _batchNumber = 0;
    private readonly DesicContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private const int _defaultBatchSize = 50;
    private readonly ILogger<SeedIso3166CountriesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<EntitySetSeedingResult> Handle(SeedIso3166CountriesRequest request, CancellationToken cancellationToken)
    {
        var result = new EntitySetSeedingResult();
        _batchNumber = 0;
        request.BatchSize ??= _defaultBatchSize;

        var tagSystem = Tags.Get(SystemTag.System);
        var nowTagOn = DateTime.UtcNow;
        var tag = new Tag
        {
            Id = Guid.CreateVersion7(),
            Name = $"Process-SeedIso3166Countries-{nowTagOn:yyyyMMddHHmmss}",
        };
        tag.SetCreatedAndModifiedBy(tagSystem, on: nowTagOn);

        _logger.LogDebug("Creating tag {TagName}", tag.Name);
        await _context.AddAsync(tag, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.Entry(tag).State = EntityState.Detached;

        _logger.LogDebug("Updating all records with IsBeingSeeded = true");
        await _context.Iso3166Countries.ExecuteUpdateAsync(c => c.SetProperty(p => p.IsBeingSeeded, p => true), cancellationToken);

        var requestStream = new Iso3166CountriesResourceStreamRequest
        {
            ClassMapType = typeof(Iso3166CountryClassMap),
            ResourceName = "Desic.Data.Resources.iso-3166-countries.csv",
        };

        var batchInserts = new List<Iso3166Country>();
        _logger.LogDebug("Starting comparison of data to determine if any records need to be inserted or udpated");
        _logger.LogDebug("Creating stream for csv resource = {ResourceName} using class map type = {ClassMapType}", requestStream.ResourceName, requestStream.ClassMapType);
        await foreach (var item in _mediator.CreateStream(requestStream, cancellationToken))
        {
            ++result.ReferenceCount;
            var countryExisting = await _context.Iso3166Countries.AsTracking().FirstOrDefaultAsync(x => x.IsoId == item.IsoId, cancellationToken);
            if (countryExisting == null)
            {
                item.Id = Guid.CreateVersion7();
                item.IsBeingSeeded = false;
                item.SetCreatedAndModifiedBy(tag);
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
                    countryExisting.SetNotDeletedAndModifiedBy(tag);
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
        _logger.LogDebug("Determining if any records need to be (soft) deleted");
        nowTagOn = DateTime.UtcNow;
        result.Deletes = await _context.Iso3166Countries
            .Where(c => c.IsBeingSeeded)
            .ExecuteUpdateAsync(c => c
                .SetProperty(p => p.IsDeleted, p => true)
                .SetProperty(p => p.DeletedById, p => tag.Id)
                .SetProperty(p => p.DeletedByTypeId, p => tag.GetEntityType().Id)
                .SetProperty(p => p.DeletedOn, p => nowTagOn)
                .SetProperty(p => p.ModifiedById, p => tag.Id)
                .SetProperty(p => p.ModifiedByTypeId, p => tag.GetEntityType().Id)
                .SetProperty(p => p.ModifiedOn, p => nowTagOn), cancellationToken);

        return result;
    }

    private async Task PerformBatchChanges(CancellationToken cancellationToken, List<Iso3166Country>? batchInserts = null, bool clearChangeTracker = false)
    {
        ++_batchNumber;
        if (batchInserts != null)
        {
            await _context.Iso3166Countries.AddRangeAsync(batchInserts, cancellationToken);
        }
        _logger.LogDebug("Saving changes for batch {BatchNumber}", _batchNumber);
        await _context.SaveChangesAsync(cancellationToken);
        batchInserts?.Clear();
        if (clearChangeTracker)
        {
            _context.ChangeTracker.Clear();
        }
    }
}
