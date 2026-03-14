using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using Desic.Extensions;
using System.Runtime.CompilerServices;

namespace Desic.Domain.Users.Test;

public static class TestUsers
{
    private const int DefaultDataCount = 10;
    private static readonly DateTime DefaultOn = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const int RandomSeed = 1;

    public static async IAsyncEnumerable<User> Generate(int count = DefaultDataCount, IReadOnlyMinimalEntity? by = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // section 1: non random (easily assertable) users
        var stop = count < NonRandomUserCount ? count : NonRandomUserCount;
        int index;
        for (index = 1; index <= stop; ++index)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            yield return NonRandomUser(index);
        }

        // section 2: random users until count is reached
        var random = new Random(RandomSeed);
        var isActive = true;
        const int maxSeconds = 725328000;
        for (var i = index; i <= count; ++i)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            var randomSeconds = random.Next(maxSeconds);
            var createdOn = DefaultOn.AddSeconds(randomSeconds);
            var modifiedOn = random.Next(2) == 0 ? createdOn : createdOn.AddSeconds(random.Next(maxSeconds - randomSeconds));
            var isDeleted = !isActive && i % 3 == 0;
            yield return NewUser(sequentialId: i, createdOn: createdOn, modifiedOn: modifiedOn, isDeleted: isDeleted, isActive: isActive, by: by);
            isActive = !isActive;
        }
    }

    #region Non Random Users
    internal const int NonRandomUserCount = 5; // keep this consistent with users in this region so the Generate method will work correctly
    public static User User01Active => NewUser(sequentialId: 1, createdOn: DefaultOn.AddDays(0), modifiedOn: DefaultOn.AddDays(0).AddMonths(1), isActive: true);
    public static User User02Inactive => NewUser(sequentialId: 2, createdOn: DefaultOn.AddDays(1), modifiedOn: DefaultOn.AddDays(1).AddMonths(1), isActive: false);
    public static User User03DeletedInactive => NewUser(sequentialId: 3, createdOn: DefaultOn.AddDays(2), modifiedOn: DefaultOn.AddDays(2).AddMonths(1), isDeleted: true, isActive: false);
    public static User User04Active => NewUser(sequentialId: 4, createdOn: DefaultOn.AddDays(3), modifiedOn: DefaultOn.AddDays(3).AddMonths(1), isActive: true);
    public static User User05Active => NewUser(sequentialId: 5, createdOn: DefaultOn.AddDays(4), modifiedOn: DefaultOn.AddDays(4).AddMonths(1), isActive: true);

    internal static User NonRandomUser(int sequentialId)
    {
        return sequentialId switch
        {
            1 => User01Active,
            2 => User02Inactive,
            3 => User03DeletedInactive,
            4 => User04Active,
            5 => User05Active,
            _ => throw new NotImplementedException($"Non random test user does not exist with sequential id: {sequentialId}")
        };
    }
    #endregion

    private static User NewUser(int sequentialId, DateTime? createdOn = null, DateTime? modifiedOn = null, bool isDeleted = false, bool isActive = true, IReadOnlyMinimalEntity? by = null)
    {
        createdOn ??= DefaultOn;
        modifiedOn ??= DefaultOn;
        by ??= SystemTags.System;
        return new User
        {
            Id = User.ClassEntityType.Id.ToIntBasedGuid(sequentialId),
            CreatedOn = createdOn ?? DefaultOn,
            CreatedById = by.Id,
            CreatedByTypeId = by.SystemEntityType.Id,
            ModifiedOn = modifiedOn ?? DefaultOn,
            ModifiedById = by.Id,
            ModifiedByTypeId = by.SystemEntityType.Id,
            IsDeleted = isDeleted,
            DeletedOn = isDeleted ? modifiedOn : null,
            DeletedById = isDeleted ? by.Id : null,
            DeletedByTypeId = isDeleted ? by.SystemEntityType.Id : null,
            Username = $"user-{sequentialId}",
            IsActive = isActive,
        };
    }
}
