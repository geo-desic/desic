using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Data.Test
{
    internal static class Users
    {
        private const int SeedDataCount = 10;
        internal static IList<User> Generate(int count = SeedDataCount)
        {
            var result = new List<User>();
            var random = new Random();
            var isActive = true;
            var entityTypeTag = EntityTypes.Get(Enums.EntityType.Tag)!;
            var entityTypeUserIdString = EntityTypes.Get(Enums.EntityType.User)!.Id.ToString();
            var tagSystem = Tags.Get(Enums.SystemTag.System)!;
            for (var i = 0; i < count; ++i)
            {
                const int maxSeconds = 725328000;
                var randomSeconds = random.Next(maxSeconds);
                var createdOn = new DateTime(2000, 1, 1).AddSeconds(randomSeconds);
                var sequentialId = i + 1;
                var sequentialIdString = $"{sequentialId}";
                var guidString = entityTypeUserIdString[..^sequentialIdString.Length] + sequentialIdString;
                isActive = !isActive;
                var isHidden = !isActive && i % 3 == 0;
                result.Add(new User
                {
                    Id = new Guid(guidString),
                    //SequentialId = sequentialId,
                    CreatedOn = createdOn,
                    CreatedById = tagSystem.Id,
                    CreatedByTypeId = entityTypeTag.Id,
                    ModifiedOn = random.Next(2) == 0 ? createdOn : createdOn.AddSeconds(random.Next(maxSeconds - randomSeconds)),
                    ModifiedById = tagSystem.Id,
                    ModifiedByTypeId = entityTypeTag.Id,
                    Username = $"user{sequentialId}",
                    IsActive = isActive,
                    IsHidden = isHidden,
                });
            }
            return result;
        }
    }
}
