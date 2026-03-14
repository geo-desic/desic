using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Extensions;
using System.Globalization;

namespace Desic.Domain.Tests.Unit.Common.Entities;

public class EntityHelpersTests
{
    private readonly TimeSpan _acceptablePrecision = TimeSpan.FromMilliseconds(500);
    private const string Unchanged = nameof(Unchanged);

    public class EntityHelpersTests001 : EntityHelpersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("2000-01-01")]
        public void SetCreatedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString)
        {
            // arrange
            var item = new Creatable()
            {
                ExtraProperty = Unchanged
            };
            var by = new MinimalEntity();
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityHelpers.SetCreatedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(Unchanged);
            // updates
            item.CreatedById.Should().Be(by.Id);
            item.CreatedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.CreatedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityHelpersTests002 : EntityHelpersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("2000-01-01")]
        public void SetModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString)
        {
            // arrange
            var unchanged = new Creatable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new Modifiable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
            };
            var by = new MinimalEntity();
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityHelpers.SetModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            // updates
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityHelpersTests003 : EntityHelpersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("2000-01-01")]
        public void SetCreatedAndModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString)
        {
            // arrange
            var item = new Modifiable()
            {
                ExtraProperty = Unchanged
            };
            var by = new MinimalEntity();
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityHelpers.SetCreatedAndModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(Unchanged);
            // updates
            item.CreatedById.Should().Be(by.Id);
            item.CreatedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.CreatedOn.Should().BeCloseTo(expectedOn, precision);
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityHelpersTests004 : EntityHelpersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("2000-01-01")]
        public void SetDeletedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString)
        {
            // arrange
            var unchanged = new Modifiable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
            };
            var by = new MinimalEntity();
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityHelpers.SetDeletedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            item.ModifiedById.Should().Be(unchanged.ModifiedById);
            item.ModifiedByTypeId.Should().Be(unchanged.ModifiedByTypeId);
            item.ModifiedOn.Should().Be(unchanged.ModifiedOn);
            // updates
            item.IsDeleted.Should().BeTrue();
            item.DeletedById.Should().Be(by.Id);
            item.DeletedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.DeletedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityHelpersTests005 : EntityHelpersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("2000-01-01")]
        public void SetDeletedAndModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString)
        {
            // arrange
            var unchanged = new Modifiable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
            };
            var by = new MinimalEntity();
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityHelpers.SetDeletedAndModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            // updates
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
            item.IsDeleted.Should().BeTrue();
            item.DeletedById.Should().Be(by.Id);
            item.DeletedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.DeletedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityHelpersTests006 : EntityHelpersTests
    {
        [Fact]
        public void SetNotDeleted_SpecifiedOnDate_UpdatesAllExpectedValues()
        {
            // arrange
            var unchanged = new Modifiable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
            };

            // act
            EntityHelpers.SetNotDeleted(item);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            item.ModifiedById.Should().Be(unchanged.ModifiedById);
            item.ModifiedByTypeId.Should().Be(unchanged.ModifiedByTypeId);
            item.ModifiedOn.Should().Be(unchanged.ModifiedOn);
            // updates
            item.IsDeleted.Should().BeFalse();
            item.DeletedById.Should().BeNull();
            item.DeletedByTypeId.Should().BeNull();
            item.DeletedOn.Should().BeNull();
        }
    }

    public class EntityHelpersTests007 : EntityHelpersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("2000-01-01")]
        public void SetNotDeletedAndModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString)
        {
            // arrange
            var unchanged = new Modifiable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
            };
            var by = new MinimalEntity();
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityHelpers.SetNotDeletedAndModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            // updates
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
            item.IsDeleted.Should().BeFalse();
            item.DeletedById.Should().BeNull();
            item.DeletedByTypeId.Should().BeNull();
            item.DeletedOn.Should().BeNull();
        }
    }

    private class Creatable : ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExtraProperty { get; set; } = Unchanged;
    }

    private class Modifiable : IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ExtraProperty { get; set; } = Unchanged;
    }

    private class SoftDeletable : ISoftDeletable, IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeletedById { get; set; }
        public Guid? DeletedByTypeId { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string ExtraProperty { get; set; } = Unchanged;
    }

    private class MinimalEntity : IReadOnlyMinimalEntity
    {
        public Guid Id { get; } = Guid.AllBitsSet;

        public SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }
}
