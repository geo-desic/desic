using Desic.Domain.Common.Entities;
using Desic.Domain.Labels;
using Desic.Infrastructure.Data.EntityTypes;
using Desic.Infrastructure.Data.Iso3166Countries;
using Desic.Infrastructure.Data.Labels;
using Desic.Infrastructure.Data.Test.Users;
using MediatR;
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

        // ordering is important due to potential entity dependencies
        await SeedEntityTypes(options: options.EntityTypes, cancellationToken: cancellationToken);
        await SeedLabels(options: options.Labels, cancellationToken: cancellationToken);

        // entity types and labels must be seeded before this by object can be created due to foriegn key and dependency requirements
        var by = await CreateBy(cancellationToken: cancellationToken);

        await SeedIso3166Countries(options: options.Iso3166Countries, by: by, cancellationToken: cancellationToken);

        // test data
        await SeedTestData(options: options.Test, by: by, cancellationToken: cancellationToken);
    }

    private async Task<IReadOnlyMinimalEntity> CreateBy(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var by = new Label
        {
            Id = Guid.CreateVersion7(),
            Name = $"Process-SeedApplicationDatabase-{now:yyyyMMddHHmmss}",
        };
        by.SetCreatedAndModifiedBy(by: SystemLabels.System, on: now);

        _logger.LogDebug("Creating label {LabelName}", by.Name);
        _context.Labels.Add(by);
        await _context.SaveChangesAsync(cancellationToken);
        return by;
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
