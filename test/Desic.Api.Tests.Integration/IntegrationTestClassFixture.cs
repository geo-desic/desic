using Microsoft.AspNetCore.Mvc.Testing;

namespace Desic.Api.Tests.Integration;

public class IntegrationTestClassFixture : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly CustomWebApplicationFactory<Program> _factory;

    public IntegrationTestClassFixture(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
