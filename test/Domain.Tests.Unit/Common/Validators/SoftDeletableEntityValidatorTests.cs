using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.Common.Validators;
using Desic.Domain.EntityTypes;
using Desic.Domain.Labels;
using FluentValidation;

namespace Desic.Domain.Tests.Unit.Common.Validators;

public class SoftDeletableEntityValidatorTests
{
    private readonly IReadOnlyMinimalEntity _by = SystemLabels.System;
    private const string ValidName = nameof(ValidName);

    public class SoftDeletableEntityValidatorTests001 : SoftDeletableEntityValidatorTests
    {
        [Theory]
        [InlineData(true, $"{nameof(SoftDeletableEntity.DeletedByName)} = {{0}} is valid", null)]
        [InlineData(true, $"{nameof(SoftDeletableEntity.DeletedByName)} = {{0}} is valid", "")]
        [InlineData(true, $"{nameof(SoftDeletableEntity.DeletedByName)} = {{0}} is valid", ValidName)]
        [InlineData(false, $"{nameof(SoftDeletableEntity.DeletedByName)} = {{0}} is too long", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length: 101
        public void Validate_OnlySpecifiedDeletedByNameValue_ExpectedResult(bool expected, string because, string? deletedByName)
        {
            // arrange
            var item = NewItem(deletedByName: deletedByName);
            var validator1 = new ReadOnlySoftDeletableValidator();
            var validator2 = new SoftDeletableEntityValidator();

            // act
            var result1 = validator1.Validate(item, o => { o.IncludeProperties(x => x.DeletedByName); });
            var result2 = validator2.Validate(item, o => { o.IncludeProperties(x => x.DeletedByName); });

            // assert
            result1.IsValid.Should().Be(expected: expected, because: because, becauseArgs: deletedByName);
            result2.IsValid.Should().Be(expected: expected, because: because, becauseArgs: deletedByName);
        }
    }

    private TestItem NewItem(string? deletedByName)
    {
        return new TestItem
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
            DeletedByName = deletedByName,
            DeletedById = deletedByName == null ? null : _by.Id,
            DeletedByTypeId = deletedByName == null ? null : _by.SystemEntityType.Id,
            DeletedOn = deletedByName == null ? null : DateTime.UtcNow,
        };
    }

    private class TestItem : SoftDeletableEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }
}
