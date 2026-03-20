using System.Data.Common;
using Xunit;

namespace Desic.Testing.Integration.Db;

public interface IDatabaseServer : IAsyncLifetime
{
    DbConnection GetConnection();
    string GetConnectionString();
}
