using Desic.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common;

public static class DbSetHelpers
{
    public static async Task<T?> GetEntityByIdAsync<T>(this DbSet<T> dbSet, Guid id, CancellationToken cancellationToken) where T : BaseEntity
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public static IQueryable<T> GetEntityByIdQuery<T>(this DbSet<T> dbSet, Guid id, CancellationToken cancellationToken) where T : BaseEntity
    {
        return dbSet.Where(x => x.Id == id);
    }
}
