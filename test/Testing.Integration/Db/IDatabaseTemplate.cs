using Xunit;

namespace Desic.Testing.Integration.Db;

public interface IDatabaseTemplate : IAsyncLifetime
{
    IDatabase NewDatabase();
}
