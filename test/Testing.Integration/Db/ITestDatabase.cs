using System.Data.Common;
using Xunit;

namespace Desic.Testing.Integration.Db;

public interface ITestDatabase : IAsyncLifetime
{
    DbConnection GetConnection();
    string GetConnectionString();
}
