using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.WebApplication;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Desic.Api.Tests.Functional;

public class FunctionalTests
{
    private readonly TestWebApplicationFactory<Program> _factory;
    protected readonly HttpClient HttpClient;

    public FunctionalTests(SeededAppDatabase testDatabase)
    {
        _factory = new TestWebApplicationFactory<Program>(testDatabase.GetConnectionString(), testDatabase.DbProvider);
        HttpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
