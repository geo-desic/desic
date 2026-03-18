using MediatR;

namespace Desic.Infrastructure.Data.SqlServer;

public class InitializeApplicationDatabaseRequest : IRequest
{
    public required string ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}
