using AwesomeAssertions;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.Labels;
using Desic.Domain.Users;
using FluentValidation;

namespace Desic.Domain.Tests.Unit.Users;

public class UserValidatorTests
{
    private readonly IReadOnlyMinimalEntity _by = SystemLabels.System;
    private const string ValidName = nameof(ValidName);

    public class UserValidatorTests001 : UserValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(User.Username)} = {{0}} is valid", ValidName)]
        [InlineData(false, $"{nameof(User.Username)} = {{0}} is empty", "")]
        [InlineData(false, $"{nameof(User.Username)} = {{0}} is too short", "aaaa")] // length: 4
        [InlineData(false, $"{nameof(User.Username)} = {{0}} is too long", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length: 101
        public void Validate_OnlySpecifiedUsernameValue_ExpectedResult(bool expected, string because, string username)
        {
            // arrange
            var item = NewItem(username: username);
            var validator1 = new ReadOnlyUserValidator();
            var validator2 = new UserValidator();

            // act
            var result1 = validator1.Validate(item, o => { o.IncludeProperties(x => x.Username); });
            var result2 = validator2.Validate(item, o => { o.IncludeProperties(x => x.Username); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: username);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: username);
        }
    }

    private User NewItem(string username)
    {
        return new User
        {
            Id = Guid.CreateVersion7(),
            CreatedByName = ValidName,
            CreatedById = _by.Id,
            CreatedByTypeId = _by.SystemEntityType.Id,
            CreatedOn = DateTime.UtcNow,
            ModifiedByName = ValidName,
            ModifiedById = _by.Id,
            ModifiedByTypeId = _by.SystemEntityType.Id,
            ModifiedOn = DateTime.UtcNow,
            Username = username,
        };
    }
}
