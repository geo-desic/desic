using AwesomeAssertions;
using Desic.Application.Common.Helpers;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Helpers;

namespace Desic.Application.Tests.Unit.Common.Helpers;

public class ByHelpersTests
{
    private const string Unchanged = nameof(Unchanged);

    public class ByHelpersTests001 : ByHelpersTests
    {
        [Fact]
        public void MapCreated_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = SystemTags.System;
            var item = new TestCreated();
            var entity = new Creatable()
            {
                CreatedById = createdBy.Id,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ByHelpers.MapCreated(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
        }
    }

    public class ByHelpersTests002 : ByHelpersTests
    {
        [Fact]
        public void MapModified_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var modifiedBy = SystemTags.System;
            var item = new TestModified();
            var entity = new Modifiable()
            {
                ModifiedById = modifiedBy.Id,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ByHelpers.MapModified(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
        }
    }

    public class ByHelpersTests003 : ByHelpersTests
    {
        [Fact]
        public void MapDeleted_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var deletedBy = SystemTags.System;
            var item = new TestSoftDeleted();
            var entity = new SoftDeletable()
            {
                DeletedById = deletedBy.Id,
                DeletedByTypeId = deletedBy.SystemEntityType.Id,
                DeletedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ByHelpers.MapDeleted(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Deleted.By.Id.Should().Be(deletedBy.Id);
            item.Deleted.By.Type.Key.Should().Be(deletedBy.SystemEntityType.Key);
            item.Deleted.By.Type.Name.Should().Be(deletedBy.SystemEntityType.Name);
            item.Deleted.On.Should().Be(entity.DeletedOn);
        }
    }

    public class ByHelpersTests004 : ByHelpersTests
    {
        [Fact]
        public void MapCreatedModified_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = SystemTags.System;
            var modifiedBy = new User { Username = "user-1" };
            var item = new TestModified();
            var entity = new Modifiable()
            {
                CreatedById = createdBy.Id,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = modifiedBy.Id,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ByHelpers.MapCreatedModified(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
        }
    }

    public class ByHelpersTests005 : ByHelpersTests
    {
        [Fact]
        public void MapCreatedModifiedDeleted_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = SystemTags.System;
            var modifiedBy = new User { Id = 1.ToGuid(), Username = "user-1" };
            var deletedBy = new User { Id = 2.ToGuid(), Username = "user-2" };
            var item = new TestSoftDeleted();
            var entity = new SoftDeletable()
            {
                CreatedById = createdBy.Id,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = modifiedBy.Id,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = deletedBy.Id,
                DeletedByTypeId = deletedBy.SystemEntityType.Id,
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ByHelpers.MapCreatedModifiedDeleted(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
            item.Deleted.By.Id.Should().Be(deletedBy.Id);
            item.Deleted.By.Type.Key.Should().Be(deletedBy.SystemEntityType.Key);
            item.Deleted.By.Type.Name.Should().Be(deletedBy.SystemEntityType.Name);
            item.Deleted.On.Should().Be(entity.DeletedOn);
        }
    }

    private class TestCreated : ICreatableDto
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
    }

    private class TestModified : IModifiableDto, ICreatableDto
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
        public RequiredOnByType Modified { get; set; } = new();
    }

    private class TestSoftDeleted : ISoftDeletableDto, IModifiableDto, ICreatableDto
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
        public RequiredOnByType Modified { get; set; } = new();
        public OptionalOnByType Deleted { get; set; } = new();
    }

    private class Creatable : ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    private class Modifiable : IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
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
    }
}
