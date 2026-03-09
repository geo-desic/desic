using Xunit;

namespace Desic.Testing.Integration.Db;

public interface ITemplateDatabase : IAsyncLifetime
{
    ITestDatabase NewTestDatabase();
}
