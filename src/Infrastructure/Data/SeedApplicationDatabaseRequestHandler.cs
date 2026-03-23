using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Labels;
using Desic.Domain.Processes;
using Desic.Infrastructure.Data.EntityTypes;
using Desic.Infrastructure.Data.Iso3166Countries;
using Desic.Infrastructure.Data.Labels;
using Desic.Infrastructure.Data.Test.Users;
using Desic.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Desic.Infrastructure.Data;

public class SeedApplicationDatabaseRequestHandler(ApplicationDbContext context, ILogger<SeedApplicationDatabaseRequestHandler> logger, IMediator mediator, IOptions<SeedApplicationDatabaseOptions> seedingOptions) : IRequestHandler<SeedApplicationDatabaseRequest>
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SeedApplicationDatabaseRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly SeedApplicationDatabaseOptions? _options = seedingOptions?.Value;

    internal const SeedApplicationDatabaseMethod DefaultSeedingMethod = SeedApplicationDatabaseMethod.Fast;

    public async Task Handle(SeedApplicationDatabaseRequest request, CancellationToken cancellationToken)
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

        var processPersisted = false;
        var process = CreateProcessUnpersisted();
        try
        {
            // ordering is important due to potential entity dependencies
            await SeedEntityTypes(options: options.EntityTypes, cancellationToken: cancellationToken);
            await SeedLabels(options: options.Labels, cancellationToken: cancellationToken);
            // note that the Process entity type and System label must be seeded before the process entity can safely be persisted due to foreign key constaints
            await PersistProcess(process: process, cancellationToken: cancellationToken);
            processPersisted = true;

            await SeedIso3166Countries(options: options.Iso3166Countries, by: process, cancellationToken: cancellationToken);

            // test data
            await SeedTestData(options: options.Test, by: process, cancellationToken: cancellationToken);
            await CompleteProcess(process: process, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            try
            {
                await FailProcess(process: process, message: ex.Message, processPersisted: processPersisted, cancellationToken: cancellationToken);
            }
            catch { /* nothing => rethrow original exception not this one */ }
            throw;
        }
    }

    private static Process CreateProcessUnpersisted()
    {
        var now = DateTime.UtcNow;
        var by = new Process
        {
            Id = Guid.CreateVersion7(),
            Name = $"Seed Application Database - {now:yyyyMMddHHmmss}",
            StartedOn = now,
        };
        by.SetCreatedAndModifiedBy(by: SystemLabels.System, on: now);
        return by;
    }

    private async Task PersistProcess(Process process, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating process: id = {ProcessId}; name = {ProcessName}", process.Id, process.Name);
        _context.Processes.Add(process);
        await _context.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    private async Task CompleteProcess(Process process, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Completing process: id = {ProcessId}; name = {ProcessName}", process.Id, process.Name);
        _context.Processes.Attach(process); // _context.ChangeTracker.Clear() has likely been called since process was created
        process.CompletedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    private async Task FailProcess(Process process, string? message, bool processPersisted, CancellationToken cancellationToken)
    {
        if (!await _context.EntityTypes.AnyAsync(x => x.Id == SystemEntityTypes.Process.Id, cancellationToken: cancellationToken)
            || !await _context.Labels.AnyAsync(x => x.Id == SystemLabels.System.Id, cancellationToken: cancellationToken))
        {
            _logger.LogDebug("Unabled to persist failed process due to non-existant required seed dependencies");
            return;
        }
        _logger.LogDebug("Failing process: id = {ProcessId}; name = {ProcessName}", process.Id, process.Name);
        if (processPersisted) _context.Processes.Attach(process); // _context.ChangeTracker.Clear() has likely been called since process was created
        else _context.Processes.Add(process);
        process.FaileddOn = DateTime.UtcNow;
        process.Message = message.Left(Process.MaxLengthMessage);
        await _context.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    private async Task SeedEntityTypes(SeedApplicationDatabaseEntityTypesOptions? options, CancellationToken cancellationToken)
    {
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding {TableName} is not enabled", nameof(_context.EntityTypes));
            return;
        }

        var request = new SeedEntityTypesRequest
        {
            By = SystemLabels.System, // unused
            Method = options?.Method ?? DefaultSeedingMethod,
        };
        await _mediator.Send(request, cancellationToken);
    }

    private async Task SeedLabels(SeedApplicationDatabaseLabelsOptions? options, CancellationToken cancellationToken)
    {
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding {TableName} is not enabled", nameof(_context.Labels));
            return;
        }

        var request = new SeedLabelsRequest
        {
            By = SystemLabels.System,
            Method = options?.Method ?? DefaultSeedingMethod,
        };
        await _mediator.Send(request, cancellationToken);
    }

    private async Task SeedIso3166Countries(SeedApplicationDatabaseIso3166CountriesOptions? options, IReadOnlyMinimalEntity by, CancellationToken cancellationToken)
    {
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding {TableName} is not enabled", nameof(_context.Iso3166Countries));
            return;
        }

        var request = new SeedIso3166CountriesRequest
        {
            By = by,
            Method = options?.Method ?? DefaultSeedingMethod,
        };
        await _mediator.Send(request, cancellationToken);
    }

    private async Task SeedTestData(SeedApplicationDatabaseTestOptions? options, IReadOnlyMinimalEntity by, CancellationToken cancellationToken)
    {
        if (!(options?.Enabled ?? false)) // unlike others if not explicitly configured seeding of test data defaults to false
        {
            _logger.LogDebug("Seeding test data is not enabled");
            return;
        }

        _logger.LogDebug("Starting seeding of test data");
        await SeedTestUsers(options: options?.Users, by: by, cancellationToken: cancellationToken);
        _logger.LogDebug("Completed seeding of test data");
    }

    private async Task SeedTestUsers(ApplicationDatabaseSeedingTestUsersOptions? options, IReadOnlyMinimalEntity by, CancellationToken cancellationToken)
    {
        if (!(options?.Enabled ?? true))
        {
            _logger.LogDebug("Seeding test {TableName} is not enabled", nameof(_context.Users));
            return;
        }

        var request = new SeedTestUsersRequest
        {
            By = by,
            Count = options?.Count,
            Method = options?.Method ?? DefaultSeedingMethod,
        };
        await _mediator.Send(request, cancellationToken);
    }
}
