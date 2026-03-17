using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Desic.Infrastructure.Tests.Unit;

public class DbContextImSqliteDependencyTests<T> : IDisposable, IAsyncDisposable where T : DbContext
{
    private readonly SqliteConnection _connection;
    protected readonly T DbContext;
    private bool _disposed = false;
    private readonly DbContextOptions<T> _options;

    public DbContextImSqliteDependencyTests(Func<DbContextOptions<T>, T> dbContextCreator)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        // this in memory sqlite database will remain as long as this connection stays open
        // so don't close it until it is automatically disposed by IDisposable or IAsyncDisposable
        _connection.Open();

        _options = new DbContextOptionsBuilder<T>().UseSqlite(_connection).Options;

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
        if (DbContext is IAsyncDisposable asyncDisposableDbContext)
        {
            await asyncDisposableDbContext.DisposeAsync().ConfigureAwait(false);
        }
        if (_connection is IAsyncDisposable asyncDisposableConnection)
        {
            await asyncDisposableConnection.DisposeAsync().ConfigureAwait(false);
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
            _connection?.Dispose();
        }
        _disposed = true;
    }
    #endregion
}
