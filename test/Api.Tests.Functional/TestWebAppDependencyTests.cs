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
        HttpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"), // to avoid https redirection warnings when using https redirection middleware
            // see: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0&pivots=xunit#client-options
        });
    }
}
