using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Extensions;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class DtoExtensionsTests
{
    private const string Unchanged = nameof(Unchanged);

    public class DtoExtensionsTests001 : DtoExtensionsTests
    {
        [Fact]
        public void MapCreated_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = SystemTags.System;
            var item = new TestCreatableDto();
            var entity = new TestCreatableEntity()
            {
                CreatedById = createdBy.Id,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            DtoExtensions.MapCreated(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
        }
    }

    public class DtoExtensionsTests002 : DtoExtensionsTests
    {
        [Fact]
        public void MapModified_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var modifiedBy = SystemTags.System;
            var item = new TestModifiableDto();
            var entity = new TestModifiableEntity()
            {
                ModifiedById = modifiedBy.Id,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            DtoExtensions.MapModified(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
        }
    }

    public class DtoExtensionsTests003 : DtoExtensionsTests
    {
        [Fact]
        public void MapDeleted_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var deletedBy = SystemTags.System;
            var item = new TestSoftDeletableDto();
            var entity = new TestSoftDeletableEntity()
            {
                DeletedById = deletedBy.Id,
                DeletedByTypeId = deletedBy.SystemEntityType.Id,
                DeletedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            DtoExtensions.MapDeleted(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Deleted.By.Id.Should().Be(deletedBy.Id);
            item.Deleted.By.Type.Key.Should().Be(deletedBy.SystemEntityType.Key);
            item.Deleted.By.Type.Name.Should().Be(deletedBy.SystemEntityType.Name);
            item.Deleted.On.Should().Be(entity.DeletedOn);
        }
    }

    public class DtoExtensionsTests004 : DtoExtensionsTests
    {
        [Fact]
        public void MapCreatedModified_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = SystemTags.System;
            var modifiedBy = new User { Username = "user-1" };
            var item = new TestModifiableDto();
            var entity = new TestModifiableEntity()
            {
                CreatedById = createdBy.Id,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = modifiedBy.Id,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            DtoExtensions.MapCreatedModified(item, entity);

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

    public class DtoExtensionsTests005 : DtoExtensionsTests
    {
        [Fact]
        public void MapCreatedModifiedDeleted_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = SystemTags.System;
            var modifiedBy = new User { Id = 1.ToGuid(), Username = "user-1" };
            var deletedBy = new User { Id = 2.ToGuid(), Username = "user-2" };
            var item = new TestSoftDeletableDto();
            var entity = new TestSoftDeletableEntity()
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
            DtoExtensions.MapCreatedModifiedDeleted(item, entity);

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

    private class TestCreatableDto : ICreatableDto
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
    }

    private class TestModifiableDto : IModifiableDto, ICreatableDto
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
        public RequiredOnByType Modified { get; set; } = new();
    }

    private class TestSoftDeletableDto : ISoftDeletableDto, IModifiableDto, ICreatableDto
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
        public RequiredOnByType Modified { get; set; } = new();
        public OptionalOnByType Deleted { get; set; } = new();
    }

    private class TestCreatableEntity : ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    private class TestModifiableEntity : IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    private class TestSoftDeletableEntity : ISoftDeletable, IModifiable, ICreatable
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
