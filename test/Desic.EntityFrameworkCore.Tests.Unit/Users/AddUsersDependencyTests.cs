using Desic.Core.Helpers;
using Desic.EntityFrameworkCore.Data;
using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Models;

namespace Desic.EntityFrameworkCore.Tests.Unit.Users;

public class AddUsersDependencyTests : DesicContextDependencyTests
{
    #region helpers
    public static User NewUser(Guid? id = null, string? username = null)
    {
        var by = Tags.Get(Enums.SystemTag.System).Id;
        var byType = EntityTypes.Get(Enums.EntityType.Tag).Id;
        var on = new DateTime(2000, 1, 1);
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
