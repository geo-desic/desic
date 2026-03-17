using Desic.Infrastructure.Data;

namespace Desic.Infrastructure.Tests.Unit;

public class ApplicationDbContextImEfCoreDependencyTests : DbContextImEfCoreDependencyTests<ApplicationDbContext>
{
    public ApplicationDbContextImEfCoreDependencyTests() : base(o => new(o)) { }
}
