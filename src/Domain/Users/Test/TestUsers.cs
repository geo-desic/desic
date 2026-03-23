using Desic.Domain.Common;
using Desic.Domain.Common.Entities;
using Desic.Domain.Labels;
using Desic.Shared.Extensions;
using System.Runtime.CompilerServices;

namespace Desic.Domain.Users.Test;

public static class TestUsers
{
    private const int DefaultDataCount = 10;
    private const string DefaultRandomUsernamePrefix = "random";
    private const string DefaultUsernamePrefix = "user";
    private static readonly DateTime DefaultOn = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Guid GuidTemplate = User.ClassEntityType.Id.ToTestData();
    private const int RandomSeed = 1;

    public static async IAsyncEnumerable<User> Generate(int count = DefaultDataCount, IReadOnlyMinimalEntity? by = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // section 1: predefined users
        var stop = count < PredefinedCount ? count : PredefinedCount;
        int index;
        for (index = 1; index <= stop; ++index)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            yield return GetPredefined(index);
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
            yield return NewUser(sequentialId: i, createdOn: createdOn, modifiedOn: modifiedOn, isDeleted: isDeleted, isActive: isActive, by: by, usernamePrefix: DefaultRandomUsernamePrefix);
            isActive = !isActive;
        }
    }

    #region Predefined (Easily Assertable) Users
    private const string DefaultPredefinedUsernamePrefix = "predefined";
    internal const int PredefinedCount = 5; // keep this consistent with users in this region so the Generate method will work correctly
    public static User User01Active => NewUser(sequentialId: 1, createdOn: DefaultOn.AddDays(0), modifiedOn: DefaultOn.AddDays(0).AddMonths(1), isActive: true, usernamePrefix: DefaultPredefinedUsernamePrefix);
    public static User User02Inactive => NewUser(sequentialId: 2, createdOn: DefaultOn.AddDays(1), modifiedOn: DefaultOn.AddDays(1).AddMonths(1), isActive: false, usernamePrefix: DefaultPredefinedUsernamePrefix);
    public static User User03DeletedInactive => NewUser(sequentialId: 3, createdOn: DefaultOn.AddDays(2), modifiedOn: DefaultOn.AddDays(2).AddMonths(1), isDeleted: true, isActive: false, usernamePrefix: DefaultPredefinedUsernamePrefix);
    public static User User04Active => NewUser(sequentialId: 4, createdOn: DefaultOn.AddDays(3), modifiedOn: DefaultOn.AddDays(3).AddMonths(1), isActive: true, usernamePrefix: DefaultPredefinedUsernamePrefix);
    public static User User05Active => NewUser(sequentialId: 5, createdOn: DefaultOn.AddDays(4), modifiedOn: DefaultOn.AddDays(4).AddMonths(1), isActive: true, usernamePrefix: DefaultPredefinedUsernamePrefix);

    internal static User GetPredefined(int sequentialId)
    {
        return sequentialId switch
        {
            1 => User01Active,
            2 => User02Inactive,
            3 => User03DeletedInactive,
            4 => User04Active,
            5 => User05Active,
            _ => throw new NotImplementedException($"Predefined item does not exist with sequential id: {sequentialId}")
        };
    }
    #endregion

    private static User NewUser(int sequentialId, DateTime? createdOn = null, DateTime? modifiedOn = null, bool isDeleted = false, bool isActive = true, IReadOnlyMinimalEntity? by = null, string usernamePrefix = DefaultUsernamePrefix)
    {
        createdOn ??= DefaultOn;
        modifiedOn ??= DefaultOn;
        by ??= SystemLabels.System;
        var namedBy = by as IReadOnlyNameable;
        return new User
        {
            Id = GuidTemplate.ToIntBasedGuid(sequentialId),
            CreatedOn = createdOn ?? DefaultOn,
            CreatedById = by.Id,
            CreatedByName = namedBy?.Name,
            CreatedByTypeId = by.SystemEntityType.Id,
            ModifiedOn = modifiedOn ?? DefaultOn,
            ModifiedById = by.Id,
            ModifiedByName = namedBy?.Name,
            ModifiedByTypeId = by.SystemEntityType.Id,
            DeletedOn = isDeleted ? modifiedOn : null,
            DeletedById = isDeleted ? by.Id : null,
            DeletedByName = isDeleted ? namedBy?.Name : null,
            DeletedByTypeId = isDeleted ? by.SystemEntityType.Id : null,
            Username = $"{usernamePrefix}-{sequentialId}",
            IsActive = isActive,
        };
    }
}
