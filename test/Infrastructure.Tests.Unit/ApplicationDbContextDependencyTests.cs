using Desic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Desic.Infrastructure.Tests.Unit;

public class ApplicationDbContextDependencyTests
{
    protected readonly ApplicationDbContext _context;
    private bool _disposed = false;
    protected readonly DbContextOptions<ApplicationDbContext> _options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString()) // unique name ensures isolation between tests
        .Options;

    public ApplicationDbContextDependencyTests()
    {
        _context = new(_options);
        _context.Database.EnsureCreated();
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
        if (_context is IAsyncDisposable asyncDisposableResource)
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
            _context?.Dispose();
        }
        _disposed = true;
    }
    #endregion
}
