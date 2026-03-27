using AwesomeAssertions;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;
using FluentValidation;

namespace Desic.Domain.Tests.Unit.EntityTypes;

public class EntityTypeValidatorTests
{
    public const string NotTested = nameof(NotTested);

    public class EntityTypeValidatorTests001 : EntityTypeValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(EntityType.Key)} = {{0}} is valid", "abcd")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is too short", "abc")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is too long", "abcde")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "abc1")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "Abcd")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "a cd")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "ab.d")]
        public void Validate_OnlySpecifiedKeyValue_ExpectedResult(bool expected, string because, string key)
        {
            // arrange
            var systemEntityType = new SystemEntityType(Id: 1.ToGuid(), Key: key, Name: NotTested);
            var entityType = systemEntityType.ToEntity();
            var validator1 = new ReadOnlyEntityTypeValidator();
            var validator2 = new SystemEntityTypeValidator();
            var validator3 = new EntityTypeValidator();

            // act
            var result1 = validator1.Validate(systemEntityType, o => { o.IncludeProperties(x => x.Key); });
            var result2 = validator2.Validate(systemEntityType, o => { o.IncludeProperties(x => x.Key); });
            var result3 = validator3.Validate(entityType, o => { o.IncludeProperties(x => x.Key); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: key);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: key);
            result3.IsValid.Should().Be(expected: expected, because: because, becauseArgs: key);
        }
    }

    public class EntityTypeValidatorTests002 : EntityTypeValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(EntityType.Name)} = {{0}} is valid", "ValidEntityType1")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} is too short", "A")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} is too long", "Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy")] // 51 characters long
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} does not start with an uppercase alphabetic character", "abc")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} contains a non alphanumeric character", "A c")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} contains a non alphanumeric character", "A.c")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} contains a non alphanumeric character", "A_c")]
        public void Validate_OnlySpecifiedNameValue_ExpectedResult(bool expected, string because, string name)
        {
            // arrange
            var systemEntityType = new SystemEntityType(Id: 1.ToGuid(), Key: NotTested, Name: name);
            var entityType = systemEntityType.ToEntity();
            var validator1 = new ReadOnlyEntityTypeValidator();
            var validator2 = new SystemEntityTypeValidator();
            var validator3 = new EntityTypeValidator();

            // act
            var result1 = validator1.Validate(systemEntityType, o => { o.IncludeProperties(x => x.Name); });
            var result2 = validator2.Validate(systemEntityType, o => { o.IncludeProperties(x => x.Name); });
            var result3 = validator3.Validate(entityType, o => { o.IncludeProperties(x => x.Name); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: name);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: name);
            result3.IsValid.Should().Be(expected: expected, because: because, becauseArgs: name);
        }
    }
}
