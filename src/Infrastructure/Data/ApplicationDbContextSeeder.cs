using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Domain.Users.Test;
using Desic.Infrastructure.Data.Common;
using Desic.Infrastructure.Data.Iso3166Countries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Desic.Infrastructure.Data;

// the bool seed argument indicates whether any store management operation was performed
// see DbContextOptionsBuilder.UseSeeding at https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.useseeding?view=efcore-10.0
internal class ApplicationDbContextSeeder(ApplicationDbContext context, bool seed, IOptions<ApplicationDbContextSeedingOptions> seedingOptions, ILogger<ApplicationDbContextSeeder> logger, IMediator mediator)
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ApplicationDbContextSeeder> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ApplicationDbContextSeedingOptions? _options = seedingOptions?.Value;
    private readonly bool _seed = seed;
    private readonly ApplicationDbContextSeedingMethod _defaultSeedingMethod = seed ? ApplicationDbContextSeedingMethod.Full : ApplicationDbContextSeedingMethod.Fast; // full when store management operations were performed, otherwise fast

    // this is needed because EF tooling does not (yet?) support async
    // see https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.useasyncseeding?view=efcore-10.0#microsoft-entityframeworkcore-dbcontextoptionsbuilder-useasyncseeding(system-func((microsoft-entityframeworkcore-dbcontext-system-boolean-system-threading-cancellationtoken-system-threading-tasks-task))):~:text=It%20is%20recomended%20to%20also%20call%20UseSeeding(Action%3CDbContext%2CBoolean%3E)%20with%20the%20same%20logic.
    public void Apply()
    {
        ApplyAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task ApplyAsync(CancellationToken cancellationToken)
    {
        if (_options == null)
        {
            _logger.LogInformation("No options were provided, using default options");
        }
        var options = _options ?? new();

        if (!options.Enabled ?? false)
        {
            _logger.LogInformation("Seeding is not enabled");
            return;
        }

        _logger.LogInformation("Store management operations were performed: {StoreManagementOperationsPerformed}", _seed);

        // ordering is important due to potential entity dependencies
        await SeedEntityTypes(cancellationToken: cancellationToken, options: options.EntityTypes);
        await SeedTags(cancellationToken: cancellationToken, options: options.Tags);
        await SeedIso3166Countries(cancellationToken: cancellationToken, options: options.Iso3166Countries);

        // test data
        await SeedTestData(cancellationToken: cancellationToken, options: options.Test);
    }

    private async Task SeedEntityTypes(CancellationToken cancellationToken, ApplicationDbContextSeedingEntityTypesOptions? options = null)
    {
        var dbSet = _context.EntityTypes;
        var tableName = nameof(_context.EntityTypes);
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding {TableName} is not enabled", tableName);
            return;
        }

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && (options?.Method ?? _defaultSeedingMethod) != ApplicationDbContextSeedingMethod.Full)
        {
            _logger.LogDebug("Skipping {TableName} as it already has records", tableName);
            return;
        }

        var result = new EntitySetSeedingResult();
        var items = SystemEntityTypes.Generate();
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
    }

    private async Task SeedTags(CancellationToken cancellationToken, ApplicationDbContextSeedingTagsOptions? options = null)
    {
        var dbSet = _context.Tags;
        var tableName = nameof(_context.Tags);
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding {TableName} is not enabled", tableName);
            return;
        }

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && (options?.Method ?? _defaultSeedingMethod) != ApplicationDbContextSeedingMethod.Full)
        {
            _logger.LogDebug("Skipping {TableName} as it already has records", tableName);
            return;
        }

        var result = new EntitySetSeedingResult();
        var items = SystemTags.Generate(); // system generated tags
        result.ReferenceCount = items.Count;
        if (!any) // fast method
        {
            await dbSet.AddRangeAsync(items, cancellationToken);
            result.Inserts = await _context.SaveChangesAsync(cancellationToken);
        }
        else // full method
        {
            var tagSystem = SystemTags.Get(SystemTag.System);
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
                        existing.SetNotDeletedAndModifiedBy(tagSystem);
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
    }

    private async Task SeedIso3166Countries(CancellationToken cancellationToken, ApplicationDbContextSeedingIso3166CountriesOptions? options = null)
    {
        var dbSet = _context.Iso3166Countries;
        var tableName = nameof(_context.Iso3166Countries);
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding {TableName} is not enabled", tableName);
            return;
        }

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && (options?.Method ?? _defaultSeedingMethod) != ApplicationDbContextSeedingMethod.Full)
        {
            _logger.LogDebug("Skipping {TableName} as it already has records", tableName);
            return;
        }

        var request = new SeedIso3166CountriesRequest();
        var result = await _mediator.Send(request, cancellationToken);
        _logger.LogInformation("Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
    }

    private async Task SeedTestData(CancellationToken cancellationToken, ApplicationDbContextSeedingTestOptions? options = null)
    {
        if (!(options?.Enabled ?? false)) // unlike others if not explicitly configured seeding of test data defaults to false
        {
            _logger.LogDebug("Seeding test data is not enabled");
            return;
        }

        _logger.LogDebug("Starting seeding of test data");
        await SeedTestUsers(cancellationToken: cancellationToken, options: options?.Users);
        _logger.LogDebug("Completed seeding of test data");
    }

    private async Task SeedTestUsers(CancellationToken cancellationToken, ApplicationDbContextSeedingTestUsersOptions? options = null)
    {
        var dbSet = _context.Users;
        var tableName = nameof(_context.Users);
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding test {TableName} is not enabled", tableName);
            return;
        }

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && (options?.Method ?? _defaultSeedingMethod) != ApplicationDbContextSeedingMethod.Full)
        {
            _logger.LogDebug("Skipping {TableName} as it already has records", tableName);
            return;
        }

        var result = new EntitySetSeedingResult();
        var items = Users.Generate(options?.Count ?? 10);
        result.ReferenceCount = items.Count;
        if (!any) // fast method
        {
            await dbSet.AddRangeAsync(items, cancellationToken);
            result.Inserts = await _context.SaveChangesAsync(cancellationToken);
        }
        else // full method
        {
            var itemsToAdd = new List<User>();
            foreach (var item in items)
            {
                var existing = await dbSet.AsTracking().FirstOrDefaultAsync(x => x.Id == item.Id, cancellationToken);
                var existingByUsername = await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Username == item.Username, cancellationToken);
                if (existing == null && existingByUsername == null) // inserts
                {
                    itemsToAdd.Add(item);
                    ++result.Inserts;
                }
                // no updates
            }
            await dbSet.AddRangeAsync(itemsToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // no deletes
        }
        _context.ChangeTracker.Clear(); // no reason to track entities after the changes have been saved

        _logger.LogInformation("Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
    }
}
