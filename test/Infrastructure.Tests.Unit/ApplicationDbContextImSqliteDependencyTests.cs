using Desic.Infrastructure.Data;

namespace Desic.Infrastructure.Tests.Unit;

public class ApplicationDbContextImSqliteDependencyTests : DbContextImSqliteDependencyTests<ApplicationDbContext>
{
    public ApplicationDbContextImSqliteDependencyTests() : base(o => new(o)) { }
}
