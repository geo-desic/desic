using Desic.Domain.Labels;
using Desic.Domain.Users;
using Desic.Infrastructure.Data;
using Desic.Shared.Extensions;

namespace Desic.Infrastructure.Tests.Unit.Data;

public class AddUsersDependencyTests : ApplicationDbContextImEfCoreDependencyTests
{
    #region helpers
    public static User NewUser(Guid? id = null, string? username = null)
    {
        var on = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        id ??= 1.ToGuid();
        username ??= "username";
        return new User
        {
            Id = id.Value,
            Username = username,
            CreatedById = SystemLabels.System.Id,
            CreatedByTypeId = SystemLabels.System.SystemEntityType.Id,
            CreatedOn = on,
            ModifiedById = SystemLabels.System.Id,
            ModifiedByTypeId = SystemLabels.System.SystemEntityType.Id,
            ModifiedOn = on,
        };
    }

    public static List<User> NewUsers(int count)
    {
        var result = new List<User>();
        for (var i = 1; i <= count; ++i)
        {
            result.Add(NewUser(i.ToGuid(), $"username{i}"));
        }
        return result;
    }

    protected List<User> AddUsers(int count)
    {
        return AddUsers(DbContext, count);
    }

    public static List<User> AddUsers(ApplicationDbContext context, int count)
    {
        var result = NewUsers(count);
        context.Users.AddRange(result);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        return result;
    }
    #endregion
}
