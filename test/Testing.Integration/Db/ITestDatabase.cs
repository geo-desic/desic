using System.Data.Common;
using Xunit;

namespace Desic.Testing.Integration.Db;

internal interface ITestDatabase : IAsyncLifetime
{
    DbConnection GetConnection();
    string GetConnectionString();
}
