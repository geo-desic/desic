using Desic.EntityFrameworkCore.Data;
using Desic.EntityFrameworkCore.Data.Resources.ClassMaps;
using Desic.EntityFrameworkCore.Data.Resources.Queries;
using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Enums;
using Desic.EntityFrameworkCore.Iso3166Countries.Models;
using Desic.EntityFrameworkCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Iso3166Countries.Commands;

internal class SeedIso3166CountriesRequestHandler(DesicContext context, ILogger<SeedIso3166CountriesRequestHandler> logger, IMediator mediator) : IRequestHandler<SeedIso3166CountriesRequest, SeedIso3166CountriesRequestHandlerResult>
{
    private int _batchNumber = 0;
    private readonly DesicContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private const int _defaultBatchSize = 100;
    private readonly ILogger<SeedIso3166CountriesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<SeedIso3166CountriesRequestHandlerResult> Handle(SeedIso3166CountriesRequest request, CancellationToken cancellationToken)
    {
        var result = new SeedIso3166CountriesRequestHandlerResult();
        _batchNumber = 0;
        request.BatchSize ??= _defaultBatchSize;

        var entityTypeTag = EntityTypes.Get(Enums.EntityType.Tag);
        var tagSystem = Tags.Get(SystemTag.System);
        var nowTagOn = DateTime.UtcNow;
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            CreatedOn = nowTagOn,
            CreatedByTypeId = entityTypeTag.Id,
            CreatedById = tagSystem.Id,
            ModifiedOn = nowTagOn,
            ModifiedByTypeId = entityTypeTag.Id,
            ModifiedById = tagSystem.Id,
            Name = $"Process-SeedIso3166Countries-{nowTagOn:yyyyMMddHHmmss}",
        };

        _logger.LogDebug("Creating tag {tagName}", tag.Name);
        await _context.AddAsync(tag, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.Entry(tag).State = EntityState.Detached;

        var requestStream = new Iso3166CountriesResourceStreamRequest
        {
            ClassMapType = typeof(Iso3166CountryClassMap),
            ResourceName = "Desic.EntityFrameworkCore.Data.Resources.iso-3166-countries.csv",
        };

        var items = new List<Iso3166Country>();
        _logger.LogDebug("Creating stream for csv resource = {resourceName} using class map type = {classMapType}", requestStream.ResourceName, requestStream.ClassMapType);
        await foreach (var item in _mediator.CreateStream(requestStream, cancellationToken))
        {
            ++result.Processed;
            var countryExisting = await _context.Iso3166Countries.AsTracking().FirstOrDefaultAsync(x => x.IsoId == item.IsoId, cancellationToken);
            if (countryExisting == null)
            {
                var now = DateTime.UtcNow;
                item.Id = Guid.NewGuid();
                item.CreatedOn = now;
                item.CreatedByTypeId = entityTypeTag.Id;
                item.CreatedById = tag.Id;
                item.ModifiedOn = now;
                item.ModifiedByTypeId = entityTypeTag.Id;
                item.ModifiedById = tag.Id;
                items.Add(item);
                ++result.Inserts;
            }
            else
            {
                // always update these so soft deletes (performed later) will work properly
                countryExisting.ModifiedOn = DateTime.UtcNow;
                countryExisting.ModifiedByTypeId = entityTypeTag.Id;
                countryExisting.ModifiedById = tag.Id;
                if (countryExisting.Alpha2 != item.Alpha2 || countryExisting.Alpha2 != item.Alpha3 || countryExisting.Name != item.Name)
                {
                    countryExisting.Alpha2 = item.Alpha2;
                    countryExisting.Alpha3 = item.Alpha3;
                    countryExisting.Name = item.Name;
                    ++result.Updates;
                    // will be updated in the context on next PerformAdditionsAndUpdates
                }
            }
            if (result.Processed % request.BatchSize == 0)
            {
                // no reason to track any added/updated items once they are saved to the context
                await PerformBatchChanges(cancellationToken: cancellationToken, itemsToAdd: items, clearChangeTracker: true);
            }
        }
        if (result.Processed % request.BatchSize != 0) // for potential partial batch at end
        {
            await PerformBatchChanges(cancellationToken: cancellationToken, itemsToAdd: items, clearChangeTracker: true);
        }

        // soft deletes
        foreach (var countryToDelete in _context.Iso3166Countries.AsTracking().Where(x => x.ModifiedById != tag.Id))
        {
            countryToDelete.IsDeleted = true;
            countryToDelete.DeletedOn = DateTime.UtcNow;
            countryToDelete.DeletedByTypeId = entityTypeTag.Id;
            countryToDelete.DeletedById = tag.Id;
            ++result.Deletes;
            if (result.Processed % request.BatchSize == 0)
            {
                await PerformBatchChanges(cancellationToken: cancellationToken, clearChangeTracker: false);
            }
        }
        if (result.Processed % request.BatchSize != 0)
        {
            await PerformBatchChanges(cancellationToken, items);
        }

        return result;
    }

    private async Task PerformBatchChanges(CancellationToken cancellationToken, List<Iso3166Country>? itemsToAdd = null, bool clearChangeTracker = false)
    {
        ++_batchNumber;
        if (itemsToAdd != null)
        {
            await _context.Iso3166Countries.AddRangeAsync(itemsToAdd, cancellationToken);
        }
        _logger.LogDebug("Saving changes for batch {batchNumber}", _batchNumber);
        await _context.SaveChangesAsync(cancellationToken);
        itemsToAdd?.Clear();
        if (clearChangeTracker)
        {
            _context.ChangeTracker.Clear();
        }
    }
}
