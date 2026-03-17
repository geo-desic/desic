using Desic.Infrastructure.Data;

namespace Desic.Infrastructure.Tests.Unit;

public class ApplicationDbContextDependencyTests : InMemoryEfCoreDependencyTests<ApplicationDbContext>
{
    public ApplicationDbContextDependencyTests() : base(o => new(o)) { }
}
