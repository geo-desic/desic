using Desic.EntityFrameworkCore.Iso3166Countries.Commands;
using Desic.EntityFrameworkCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Data;

internal class DesicContextSeeder(DesicContext context, IConfiguration config, ILogger<DesicContextSeeder> logger, IMediator mediator)
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly DesicContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<DesicContextSeeder> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    private const string ConfigKeyBase = "Databases:Desic:Seeding";
    private const string ConfigKeySeedingEnabled = $"{ConfigKeyBase}:Enabled";
    private const string ConfigKeyIso3166CountriesBase = $"{ConfigKeyBase}:Iso3166Countries";
    private const string ConfigKeyIso3166CountriesEnabled = $"{ConfigKeyIso3166CountriesBase}:Enabled";
    private const string ConfigKeyTestUsersBase = $"{ConfigKeyUsersBase}:Test";
    private const string ConfigKeyTestUsersCount = $"{ConfigKeyTestUsersBase}:Count";
    private const string ConfigKeyTestUsersEnabled = $"{ConfigKeyTestUsersBase}:Enabled";
    private const string ConfigKeyUsersBase = $"{ConfigKeyBase}:Users";
    private const string ConfigKeyUsersEnabled = $"{ConfigKeyUsersBase}:Enabled";

    // this is needed because EF tooling does not (yet?) support async
    public void Apply()
    {
        ApplyAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task ApplyAsync(CancellationToken cancellationToken)
    {
        if (!_config.GetValue(ConfigKeySeedingEnabled, false))
        {
            _logger.LogWarning("Configuration value is not enabled: {configKey}", ConfigKeySeedingEnabled);
            return;
        }

        if (!_context.EntityTypes.Any())
        {
            var items = EntityTypes.Generate();
            _context.EntityTypes.AddRange(items);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded {count} EntityTypes", items.Count);
            _context.ChangeTracker.Clear(); // EF foreign key issues occur if this is not done after adding the entity types to the db
        }

        if (!_context.Tags.Any())
        {
            var items = Tags.Generate();
            _context.Tags.AddRange(items);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded {count} Tags", items.Count);
        }

        if (_config.GetValue(ConfigKeyUsersEnabled, true))
        {
            if (_config.GetValue(ConfigKeyTestUsersEnabled, false))
            {
                if (!_context.Users.Any())
                {
                    var items = Test.Users.Generate(_config.GetValue(ConfigKeyTestUsersCount, 10));
                    _context.Users.AddRange(items);
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded {count} Users", items.Count);
                }
            }
            else
            {
                _logger.LogDebug("No test Users seeded because configuration value is not enabled: {configKey}", ConfigKeyTestUsersEnabled);
            }
        }
        else
        {
            _logger.LogDebug("No Users seeded because configuration value is not enabled: {configKey}", ConfigKeyUsersEnabled);
        }

        if (_config.GetValue(ConfigKeyIso3166CountriesEnabled, true))
        {
            var request = new SeedIso3166CountriesRequest();
            var result = await _mediator.Send(request, cancellationToken);
            _logger.LogInformation("Seeded Iso3166Countries: processed = {countProcessed}, inserts = {countInserts}, updates = {countUpdates}, deletes = {countDeletes}", result.Processed, result.Inserts, result.Updates, result.Deletes);
        }
        else
        {
            _logger.LogDebug("No Iso3166Countries seeded because configuration value is not enabled: {configKey}", ConfigKeyIso3166CountriesEnabled);
        }
    }
}
