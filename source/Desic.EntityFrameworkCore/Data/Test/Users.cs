using Desic.Core.Helpers;
using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Data.Test;

internal static class Users
{
    private const int DefaultDataCount = 10;
    private const int RandomSeed = 1;
    internal static IList<User> Generate(int count = DefaultDataCount)
    {
        var result = new List<User>();
        var random = new Random(RandomSeed);
        var isActive = true;
        var entityTypeTag = EntityTypes.Get(Enums.EntityType.Tag);
        var entityTypeUserId = EntityTypes.Get(Enums.EntityType.User).Id;
        var tagSystem = Tags.Get(Enums.SystemTag.System);
        for (var i = 0; i < count; ++i)
        {
            const int maxSeconds = 725328000;
            var randomSeconds = random.Next(maxSeconds);
            var createdOn = new DateTime(2000, 1, 1).AddSeconds(randomSeconds);
            var modifiedOn = random.Next(2) == 0 ? createdOn : createdOn.AddSeconds(random.Next(maxSeconds - randomSeconds));
            var sequentialId = i + 1;
            var isDeleted = !isActive && i % 3 == 0;
            result.Add(new User
            {
                Id = entityTypeUserId.ToIntBasedGuid(sequentialId),
                CreatedOn = createdOn,
                CreatedById = tagSystem.Id,
                CreatedByTypeId = entityTypeTag.Id,
                ModifiedOn = modifiedOn,
                ModifiedById = tagSystem.Id,
                ModifiedByTypeId = entityTypeTag.Id,
                IsDeleted = isDeleted,
                DeletedOn = isDeleted ? modifiedOn : null,
                DeletedById = isDeleted ? tagSystem.Id : null,
                DeletedByTypeId = isDeleted ? entityTypeTag.Id : null,
                Username = $"user-{sequentialId}",
                IsActive = isActive,
            });
            isActive = !isActive;
        }
        return result;
    }
}
