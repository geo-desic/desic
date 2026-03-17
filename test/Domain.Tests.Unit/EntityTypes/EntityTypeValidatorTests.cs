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
        [InlineData(true, $"{nameof(EntityType.Key)} = {{0}} is valid", "abcd", "abcd")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is too short", "abc", "abc")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is too long", "abcde", "abcde")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "abc1", "abc1")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "Abcd", "Abcd")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "a cd", "a cd")]
        [InlineData(false, $"{nameof(EntityType.Key)} = {{0}} is not all lowercase alphabetic characters", "ab.d", "ab.d")]
        public void Validate_OnlySpecifiedKeyValue_ExpectedResult(bool expected, string because, string becauseArgs, string key)
        {
            // arrange
            var systemEntityType = new SystemEntityType(Id: 1.ToGuid(), Key: key, Name: NotTested);
            var entityType = systemEntityType.ToEntity();
            var validator1 = new SystemEntityTypeValidator();
            var validator2 = new EntityTypeValidator();

            // act
            var result1 = validator1.Validate(systemEntityType, o => { o.IncludeProperties(x => x.Key); });
            var result2 = validator2.Validate(entityType, o => { o.IncludeProperties(x => x.Key); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: becauseArgs);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: becauseArgs);
        }
    }

    public class EntityTypeValidatorTests002 : EntityTypeValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(EntityType.Name)} = {{0}} is valid", "ValidEntityType1", "ValidEntityType1")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} is too short", "A", "A")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} is too long", "Abcdefghijklmnopqrstuvwxyz", "Abcdefghijklmnopqrstuvwxyz")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} does not start with an uppercase alphabetic character", "abc", "abc")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} contains a non alphanumeric character", "A c", "A c")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} contains a non alphanumeric character", "A.c", "A.c")]
        [InlineData(false, $"{nameof(EntityType.Name)} = {{0}} contains a non alphanumeric character", "A_c", "A_c")]
        public void Validate_OnlySpecifiedNameValue_ExpectedResult(bool expected, string because, string becauseArgs, string name)
        {
            // arrange
            var systemEntityType = new SystemEntityType(Id: 1.ToGuid(), Key: NotTested, Name: name);
            var entityType = systemEntityType.ToEntity();
            var validator1 = new SystemEntityTypeValidator();
            var validator2 = new EntityTypeValidator();

            // act
            var result1 = validator1.Validate(systemEntityType, o => { o.IncludeProperties(x => x.Name); });
            var result2 = validator2.Validate(entityType, o => { o.IncludeProperties(x => x.Name); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: becauseArgs);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: becauseArgs);
        }
    }
}
