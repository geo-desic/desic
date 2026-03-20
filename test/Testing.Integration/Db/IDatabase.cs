using Xunit;

namespace Desic.Testing.Integration.Db;

public interface IDatabase : IDatabaseServer, IAsyncLifetime
{
    string DatabaseName { get; }
}
