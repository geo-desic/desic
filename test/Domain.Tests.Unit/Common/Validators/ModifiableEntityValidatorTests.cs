using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.Common.Validators;
using Desic.Domain.EntityTypes;
using Desic.Domain.Labels;
using FluentValidation;

namespace Desic.Domain.Tests.Unit.Common.Validators;

public class ModifiableEntityValidatorTests
{
    private readonly IReadOnlyMinimalEntity _by = SystemLabels.System;
    private const string ValidName = nameof(ValidName);

    public class ModifiableEntityValidatorTests001 : ModifiableEntityValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(ModifiableEntity.ModifiedByName)} = {{0}} is valid", ValidName)]
        [InlineData(false, $"{nameof(ModifiableEntity.ModifiedByName)} = {{0}} is empty", "")]
        [InlineData(false, $"{nameof(ModifiableEntity.ModifiedByName)} = {{0}} is too long", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length: 101 characters
        public void Validate_OnlySpecifiedModifiedByNameValue_ExpectedResult(bool expected, string because, string modifiedByName)
        {
            // arrange
            var item = NewItem(modifiedByName: modifiedByName);
            var validator1 = new ModifiableValidator();
            var validator2 = new ModifiableEntityValidator();

            // act
            var result1 = validator1.Validate(item, o => { o.IncludeProperties(x => x.ModifiedByName); });
            var result2 = validator2.Validate(item, o => { o.IncludeProperties(x => x.ModifiedByName); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: modifiedByName);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: modifiedByName);
        }
    }

    private TestItem NewItem(string modifiedByName)
    {
        return new TestItem
        {
            Id = Guid.CreateVersion7(),
            CreatedByName = ValidName,
            CreatedById = _by.Id,
            CreatedByTypeId = _by.SystemEntityType.Id,
            CreatedOn = DateTime.UtcNow,
            ModifiedByName = modifiedByName,
            ModifiedById = _by.Id,
            ModifiedByTypeId = _by.SystemEntityType.Id,
            ModifiedOn = DateTime.UtcNow,
        };
    }

    private class TestItem : ModifiableEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }
}
