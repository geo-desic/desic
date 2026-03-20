using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Desic.Api.Tests.Functional;

public class TestWebAppDependencyTests
{
    private readonly TestWebApplicationFactory<Program> _factory;
    protected readonly HttpClient HttpClient;

    public TestWebAppDependencyTests(SeededAppDatabase testDatabase)
    {
        _factory = new TestWebApplicationFactory<Program>(testDatabase.GetConnectionString(), testDatabase.DbProvider);
        HttpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
