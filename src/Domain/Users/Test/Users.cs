using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using Desic.Helpers;

namespace Desic.Domain.Users.Test;

public static class Users
{
    private const int DefaultDataCount = 10;
    private const int RandomSeed = 1;
    public static IList<User> Generate(int count = DefaultDataCount, IReadOnlyMinimalEntity? by = null)
    {
        by ??= SystemTags.System;
        var result = new List<User>();
        var random = new Random(RandomSeed);
        var isActive = true;
        for (var i = 0; i < count; ++i)
        {
            const int maxSeconds = 725328000;
            var randomSeconds = random.Next(maxSeconds);
            var createdOn = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(randomSeconds);
            var modifiedOn = random.Next(2) == 0 ? createdOn : createdOn.AddSeconds(random.Next(maxSeconds - randomSeconds));
            var sequentialId = i + 1;
            var isDeleted = !isActive && i % 3 == 0;
            result.Add(new User
            {
                Id = User.ClassEntityType.Id.ToIntBasedGuid(sequentialId),
                CreatedOn = createdOn,
                CreatedById = by.Id,
                CreatedByTypeId = by.SystemEntityType.Id,
                ModifiedOn = modifiedOn,
                ModifiedById = by.Id,
                ModifiedByTypeId = by.SystemEntityType.Id,
                IsDeleted = isDeleted,
                DeletedOn = isDeleted ? modifiedOn : null,
                DeletedById = isDeleted ? by.Id : null,
                DeletedByTypeId = isDeleted ? by.SystemEntityType.Id : null,
                Username = $"user-{sequentialId}",
                IsActive = isActive,
            });
            isActive = !isActive;
        }
        return result;
    }
}
