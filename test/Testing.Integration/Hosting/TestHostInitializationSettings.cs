using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Testing.Integration.Hosting;

public class TestHostInitializationSettings
{
    public string[]? CommandLineArgs { get; set; }
    public string? ConnectionString { get; set; }
    public DbProvider? DbProvider { get; set; }
    public string? EnvironmentName { get; set; } = Constants.TestEnvironmentName;
    public Action<IServiceCollection, IConfiguration>? RegisterServices { get; set; }
}
