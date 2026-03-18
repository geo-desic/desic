using Desic.Application.Common;
using Desic.Domain.Users;
using Desic.Domain.Users.Test;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Infrastructure.Data.Test.Users;

public class SeedTestUsersRequestHandler(ApplicationDbContext context, ILogger<SeedTestUsersRequestHandler> logger) : IRequestHandler<SeedTestUsersRequest, SeedTestUsersResult>
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SeedTestUsersRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const int DefaultCount = 10;
    private const int LogEventId = LogEvents.SeedTestUsers;

    public async Task<SeedTestUsersResult> Handle(SeedTestUsersRequest request, CancellationToken cancellationToken)
    {
        var result = new SeedTestUsersResult();
        var dbSet = _context.Users;
        var tableName = nameof(_context.Users);

        var any = await dbSet.AnyAsync(cancellationToken);
        if (any && request.Method != SeedApplicationDatabaseMethod.Full)
        {
            _logger.LogDebug(LogEventId, "Skipping {TableName} as it already has records", tableName);
            return result;
        }

        var items = await TestUsers.Generate(request.Count ?? DefaultCount, by: request.By, cancellationToken: cancellationToken).ToListAsync(cancellationToken: cancellationToken);
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

        _logger.LogInformation(LogEventId, "Seeded {TableName}: reference count = {CountReference}, inserts = {CountInserts}, updates = {CountUpdates}, deletes = {CountDeletes}", tableName, result.ReferenceCount, result.Inserts, result.Updates, result.Deletes);
        return result;
    }
}
