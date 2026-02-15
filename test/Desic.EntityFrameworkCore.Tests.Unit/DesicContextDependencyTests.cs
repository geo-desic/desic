using Desic.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.EntityFrameworkCore.Tests.Unit;

public class DesicContextDependencyTests
{
    protected readonly DesicContext _context;
    private bool _disposed = false;
    protected readonly DbContextOptions<DesicContext> _options = new DbContextOptionsBuilder<DesicContext>()
        .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString()) // unique name ensures isolation between tests
        .Options;

    public DesicContextDependencyTests()
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
