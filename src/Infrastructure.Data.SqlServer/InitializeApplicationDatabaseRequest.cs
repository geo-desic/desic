using DispatchR.Abstractions.Send;

namespace Desic.Infrastructure.Data.SqlServer;

public sealed class InitializeApplicationDatabaseRequest : IRequest<InitializeApplicationDatabaseRequest, Task>
{
    public required string ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}
