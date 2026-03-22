using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;
using System.Globalization;

namespace Desic.Domain.Tests.Unit.Common.Entities;

public class EntityExtensionsTests
{
    private readonly TimeSpan _acceptablePrecision = TimeSpan.FromMilliseconds(500);
    private const string ExpectedByName = nameof(ExpectedByName);
    private const string Unchanged = nameof(Unchanged);

    public class EntityExtensionsTests001 : EntityExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("2000-01-01", false)]
        public void SetCreatedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString, bool byNamedEntity)
        {
            // arrange
            var unchanged = new Creatable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new Creatable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
            };
            IReadOnlyMinimalEntity by = byNamedEntity ? new ByNamedEntity() : new ByUnnamedEntity();
            var byNamed = by as IReadOnlyNameable;
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityExtensions.SetCreatedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(Unchanged);
            // updates
            item.CreatedById.Should().Be(by.Id);
            item.CreatedByName.Should().Be(byNamed?.Name ?? unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.CreatedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityExtensionsTests002 : EntityExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("2000-01-01", false)]
        public void SetModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString, bool byNamedEntity)
        {
            // arrange
            var unchanged = new Modifiable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByName = 2.ToGuid().ToString(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new Modifiable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByName = unchanged.ModifiedByName,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
            };
            IReadOnlyMinimalEntity by = byNamedEntity ? new ByNamedEntity() : new ByUnnamedEntity();
            var byNamed = by as IReadOnlyNameable;
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityExtensions.SetModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByName.Should().Be(unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            // updates
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByName.Should().Be(byNamed?.Name ?? unchanged.ModifiedByName);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityExtensionsTests003 : EntityExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("2000-01-01", false)]
        public void SetCreatedAndModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString, bool byNamedEntity)
        {
            // arrange
            var unchanged = new Modifiable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByName = 2.ToGuid().ToString(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new Modifiable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByName = unchanged.ModifiedByName,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
            };
            IReadOnlyMinimalEntity by = byNamedEntity ? new ByNamedEntity() : new ByUnnamedEntity();
            var byNamed = by as IReadOnlyNameable;
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityExtensions.SetCreatedAndModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(Unchanged);
            // updates
            item.CreatedById.Should().Be(by.Id);
            item.CreatedByName.Should().Be(byNamed?.Name ?? unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.CreatedOn.Should().BeCloseTo(expectedOn, precision);
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByName.Should().Be(byNamed?.Name ?? unchanged.ModifiedByName);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityExtensionsTests004 : EntityExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("2000-01-01", false)]
        public void SetDeletedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString, bool byNamedEntity)
        {
            // arrange
            var unchanged = new SoftDeletable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByName = 2.ToGuid().ToString(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 3.ToGuid(),
                DeletedByName = 3.ToGuid().ToString(),
                DeletedByTypeId = 3.ToGuid(),
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByName = unchanged.ModifiedByName,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
                DeletedById = unchanged.DeletedById,
                DeletedByName = unchanged.DeletedByName,
                DeletedByTypeId = unchanged.DeletedByTypeId,
                DeletedOn = unchanged.DeletedOn,
            };
            IReadOnlyMinimalEntity by = byNamedEntity ? new ByNamedEntity() : new ByUnnamedEntity();
            var byNamed = by as IReadOnlyNameable;
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityExtensions.SetDeletedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByName.Should().Be(unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            item.ModifiedById.Should().Be(unchanged.ModifiedById);
            item.ModifiedByName.Should().Be(unchanged.ModifiedByName);
            item.ModifiedByTypeId.Should().Be(unchanged.ModifiedByTypeId);
            item.ModifiedOn.Should().Be(unchanged.ModifiedOn);
            // updates
            item.IsDeleted.Should().BeTrue();
            item.DeletedById.Should().Be(by.Id);
            item.DeletedByName.Should().Be(byNamed?.Name ?? unchanged.DeletedByName);
            item.DeletedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.DeletedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityExtensionsTests005 : EntityExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("2000-01-01", false)]
        public void SetDeletedAndModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString, bool byNamedEntity)
        {
            // arrange
            var unchanged = new SoftDeletable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByName = 2.ToGuid().ToString(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 3.ToGuid(),
                DeletedByName = 3.ToGuid().ToString(),
                DeletedByTypeId = 3.ToGuid(),
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByName = unchanged.ModifiedByName,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
                DeletedById = unchanged.DeletedById,
                DeletedByName = unchanged.DeletedByName,
                DeletedByTypeId = unchanged.DeletedByTypeId,
                DeletedOn = unchanged.DeletedOn,
            };
            IReadOnlyMinimalEntity by = byNamedEntity ? new ByNamedEntity() : new ByUnnamedEntity();
            var byNamed = by as IReadOnlyNameable;
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityExtensions.SetDeletedAndModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByName.Should().Be(unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            // updates
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByName.Should().Be(byNamed?.Name ?? unchanged.ModifiedByName);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
            item.IsDeleted.Should().BeTrue();
            item.DeletedById.Should().Be(by.Id);
            item.DeletedByName.Should().Be(byNamed?.Name ?? unchanged.DeletedByName);
            item.DeletedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.DeletedOn.Should().BeCloseTo(expectedOn, precision);
        }
    }

    public class EntityExtensionsTests006 : EntityExtensionsTests
    {
        [Fact]
        public void SetNotDeleted_SpecifiedOnDate_UpdatesAllExpectedValues()
        {
            // arrange
            var unchanged = new SoftDeletable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByName = 2.ToGuid().ToString(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 3.ToGuid(),
                DeletedByName = 3.ToGuid().ToString(),
                DeletedByTypeId = 3.ToGuid(),
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByName = unchanged.ModifiedByName,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
                DeletedById = unchanged.DeletedById,
                DeletedByName = unchanged.DeletedByName,
                DeletedByTypeId = unchanged.DeletedByTypeId,
                DeletedOn = unchanged.DeletedOn,
            };

            // act
            EntityExtensions.SetNotDeleted(item);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByName.Should().Be(unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            item.ModifiedById.Should().Be(unchanged.ModifiedById);
            item.ModifiedByName.Should().Be(unchanged.ModifiedByName);
            item.ModifiedByTypeId.Should().Be(unchanged.ModifiedByTypeId);
            item.ModifiedOn.Should().Be(unchanged.ModifiedOn);
            // updates
            item.IsDeleted.Should().BeFalse();
            item.DeletedById.Should().BeNull();
            item.DeletedByName.Should().BeNull();
            item.DeletedByTypeId.Should().BeNull();
            item.DeletedOn.Should().BeNull();
        }
    }

    public class EntityExtensionsTests007 : EntityExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("2000-01-01", false)]
        public void SetNotDeletedAndModifiedBy_SpecifiedOnDate_UpdatesAllExpectedValues(string? onString, bool byNamedEntity)
        {
            // arrange
            var unchanged = new SoftDeletable()
            {
                ExtraProperty = Unchanged,
                CreatedById = 1.ToGuid(),
                CreatedByName = 1.ToGuid().ToString(),
                CreatedByTypeId = 1.ToGuid(),
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 2.ToGuid(),
                ModifiedByName = 2.ToGuid().ToString(),
                ModifiedByTypeId = 2.ToGuid(),
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 3.ToGuid(),
                DeletedByName = 3.ToGuid().ToString(),
                DeletedByTypeId = 3.ToGuid(),
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };
            var item = new SoftDeletable()
            {
                ExtraProperty = unchanged.ExtraProperty,
                CreatedById = unchanged.CreatedById,
                CreatedByName = unchanged.CreatedByName,
                CreatedByTypeId = unchanged.CreatedByTypeId,
                CreatedOn = unchanged.CreatedOn,
                ModifiedById = unchanged.ModifiedById,
                ModifiedByName = unchanged.ModifiedByName,
                ModifiedByTypeId = unchanged.ModifiedByTypeId,
                ModifiedOn = unchanged.ModifiedOn,
                DeletedById = unchanged.DeletedById,
                DeletedByName = unchanged.DeletedByName,
                DeletedByTypeId = unchanged.DeletedByTypeId,
                DeletedOn = unchanged.DeletedOn,
            };
            IReadOnlyMinimalEntity by = byNamedEntity ? new ByNamedEntity() : new ByUnnamedEntity();
            var byNamed = by as IReadOnlyNameable;
            DateTime? on = onString != null ? DateTime.Parse(onString, CultureInfo.InvariantCulture) : null;
            var precision = on.HasValue ? TimeSpan.FromMilliseconds(0) : _acceptablePrecision; // exact precision when a date is provided
            var expectedOn = on ?? DateTime.UtcNow;

            // act
            EntityExtensions.SetNotDeletedAndModifiedBy(item, by: by, on: on);

            // assert
            // unchanged
            item.ExtraProperty.Should().Be(unchanged.ExtraProperty);
            item.CreatedById.Should().Be(unchanged.CreatedById);
            item.CreatedByName.Should().Be(unchanged.CreatedByName);
            item.CreatedByTypeId.Should().Be(unchanged.CreatedByTypeId);
            item.CreatedOn.Should().Be(unchanged.CreatedOn);
            // updates
            item.ModifiedById.Should().Be(by.Id);
            item.ModifiedByName.Should().Be(byNamed?.Name ?? unchanged.ModifiedByName);
            item.ModifiedByTypeId.Should().Be(by.SystemEntityType.Id);
            item.ModifiedOn.Should().BeCloseTo(expectedOn, precision);
            item.IsDeleted.Should().BeFalse();
            item.DeletedById.Should().BeNull();
            item.DeletedByName.Should().BeNull();
            item.DeletedByTypeId.Should().BeNull();
            item.DeletedOn.Should().BeNull();
        }
    }

    private class Creatable : ICreatable
    {
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExtraProperty { get; set; } = Unchanged;
    }

    private class Modifiable : IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ExtraProperty { get; set; } = Unchanged;
    }

    private class SoftDeletable : ISoftDeletable, IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid? DeletedById { get; set; }
        public string? DeletedByName { get; set; }
        public Guid? DeletedByTypeId { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted => DeletedOn.HasValue;
        public string ExtraProperty { get; set; } = Unchanged;
    }

    private class ByUnnamedEntity : IReadOnlyMinimalEntity
    {
        public SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
        public Guid Id { get; } = Guid.AllBitsSet;
    }

    private class ByNamedEntity : IReadOnlyMinimalEntity, IReadOnlyNameable
    {
        public SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
        public Guid Id { get; } = Guid.AllBitsSet;

        public string Name => ExpectedByName;
    }
}
