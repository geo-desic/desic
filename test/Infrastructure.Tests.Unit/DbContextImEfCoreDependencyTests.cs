using Microsoft.EntityFrameworkCore;

namespace Desic.Infrastructure.Tests.Unit;

public class DbContextImEfCoreDependencyTests<T> : IDisposable, IAsyncDisposable where T : DbContext
{
    protected readonly T DbContext;
    private bool _disposed = false;
    private readonly DbContextOptions<T> _options = new DbContextOptionsBuilder<T>()
        .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString()) // unique name ensures isolation between tests
        .Options;

    public DbContextImEfCoreDependencyTests(Func<DbContextOptions<T>, T> dbContextCreator)
    {
        DbContext = dbContextCreator(_options);
        DbContext.Database.EnsureCreated();
    }

    #region disposable
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed) return;
        if (DbContext is IAsyncDisposable asyncDisposableResource)
        {
            await asyncDisposableResource.DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            DbContext?.Dispose();
        }
        _disposed = true;
    }
    #endregion
}
