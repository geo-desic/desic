using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.Common.Validators;
using Desic.Domain.EntityTypes;
using Desic.Domain.Labels;
using FluentValidation;

namespace Desic.Domain.Tests.Unit.Common.Validators;

public class CreatableEntityValidatorTests
{
    private readonly IReadOnlyMinimalEntity _by = SystemLabels.System;
    private const string ValidName = nameof(ValidName);

    public class CreatableEntityValidatorTests001 : CreatableEntityValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(CreatableEntity.CreatedByName)} = {{0}} is valid", ValidName)]
        [InlineData(false, $"{nameof(CreatableEntity.CreatedByName)} = {{0}} is empty", "")]
        [InlineData(false, $"{nameof(CreatableEntity.CreatedByName)} = {{0}} is too long", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length: 101 characters
        public void Validate_OnlySpecifiedCreatedByNameValue_ExpectedResult(bool expected, string because, string createdByName)
        {
            // arrange
            var item = NewItem(createdByName: createdByName);
            var validator1 = new ReadOnlyCreatableValidator();
            var validator2 = new CreatableEntityValidator();

            // act
            var result1 = validator1.Validate(item, o => { o.IncludeProperties(x => x.CreatedByName); });
            var result2 = validator2.Validate(item, o => { o.IncludeProperties(x => x.CreatedByName); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: createdByName);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: createdByName);
        }
    }

    private TestItem NewItem(string createdByName)
    {
        return new TestItem
        {
            Id = Guid.CreateVersion7(),
            CreatedByName = createdByName,
            CreatedById = _by.Id,
            CreatedByTypeId = _by.SystemEntityType.Id,
            CreatedOn = DateTime.UtcNow,
        };
    }

    private class TestItem : CreatableEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }
}
