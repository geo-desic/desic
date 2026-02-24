using Desic.Core.EntityTypes;
using Desic.Core.Tags;
using Desic.Core.Users;
using Desic.EntityFrameworkCore.Data;
using Desic.Helpers;

namespace Desic.EntityFrameworkCore.Tests.Unit.Users;

public class AddUsersDependencyTests : DesicContextDependencyTests
{
    #region helpers
    public static User NewUser(Guid? id = null, string? username = null)
    {
        var by = SystemTags.Get(SystemTag.System).Id;
        var byType = SystemEntityTypes.Get(SystemEntityType.Tag).Id;
        var on = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        id ??= 1.ToGuid();
        username ??= "username";
        return new User
        {
            Id = id.Value,
            Username = username,
            CreatedById = by,
            CreatedByTypeId = byType,
            CreatedOn = on,
            ModifiedById = by,
            ModifiedByTypeId = byType,
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
        return AddUsers(_context, count);
    }

    public static List<User> AddUsers(DesicContext context, int count)
    {
        var result = NewUsers(count);
        context.Users.AddRange(result);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        return result;
    }
    #endregion
}
