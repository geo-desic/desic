using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.Labels;
using Desic.Domain.Users;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class ModelExtensionsTests
{
    private readonly SystemLabel _by = SystemLabels.System;
    private const string Unchanged = nameof(Unchanged);

    public class ModelExtensionsTests001 : ModelExtensionsTests
    {
        [Fact]
        public void MapId_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var item = new TestIdModel();
            var entity = new TestIdEntity()
            {
                Id = 1.ToGuid(),
            };

            // act
            ModelExtensions.MapId(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Id.Should().Be(entity.Id);
        }
    }

    public class ModelExtensionsTests002 : ModelExtensionsTests
    {
        [Fact]
        public void MapCreated_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = _by;
            var item = new TestCreatableModel();
            var entity = new TestCreatableEntity()
            {
                CreatedById = createdBy.Id,
                CreatedByName = createdBy.Name,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ModelExtensions.MapCreated(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Name.Should().Be(createdBy.Name);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
        }
    }

    public class ModelExtensionsTests003 : ModelExtensionsTests
    {
        [Fact]
        public void MapModified_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var modifiedBy = _by;
            var item = new TestModifiableModel();
            var entity = new TestModifiableEntity()
            {
                ModifiedById = modifiedBy.Id,
                ModifiedByName = modifiedBy.Name,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ModelExtensions.MapModified(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Name.Should().Be(modifiedBy.Name);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
        }
    }

    public class ModelExtensionsTests004 : ModelExtensionsTests
    {
        [Fact]
        public void MapDeleted_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var deletedBy = _by;
            var item = new TestSoftDeletableModel();
            var entity = new TestSoftDeletableEntity()
            {
                DeletedById = deletedBy.Id,
                DeletedByName = deletedBy.Name,
                DeletedByTypeId = deletedBy.SystemEntityType.Id,
                DeletedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ModelExtensions.MapDeleted(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Deleted.By.Id.Should().Be(deletedBy.Id);
            item.Deleted.By.Name.Should().Be(deletedBy.Name);
            item.Deleted.By.Type.Key.Should().Be(deletedBy.SystemEntityType.Key);
            item.Deleted.By.Type.Name.Should().Be(deletedBy.SystemEntityType.Name);
            item.Deleted.On.Should().Be(entity.DeletedOn);
        }
    }

    public class ModelExtensionsTests005 : ModelExtensionsTests
    {
        [Fact]
        public void MapCreatedModified_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = _by;
            var modifiedBy = new User { Username = "user-1" };
            var item = new TestModifiableModel();
            var entity = new TestModifiableEntity()
            {
                CreatedById = createdBy.Id,
                CreatedByName = createdBy.Name,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = modifiedBy.Id,
                ModifiedByName = ((IReadOnlyNameable)modifiedBy).Name,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ModelExtensions.MapCreatedModified(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Name.Should().Be(createdBy.Name);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Name.Should().Be(((IReadOnlyNameable)modifiedBy).Name);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
        }
    }

    public class ModelExtensionsTests006 : ModelExtensionsTests
    {
        [Fact]
        public void MapCreatedModifiedDeleted_SpecifiedModels_PerformsExpectedMapping()
        {
            // arrange
            var createdBy = _by;
            var modifiedBy = new User { Id = 1.ToGuid(), Username = "user-1" };
            var deletedBy = new User { Id = 2.ToGuid(), Username = "user-2" };
            var item = new TestSoftDeletableModel();
            var entity = new TestSoftDeletableEntity()
            {
                CreatedById = createdBy.Id,
                CreatedByName = createdBy.Name,
                CreatedByTypeId = createdBy.SystemEntityType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = modifiedBy.Id,
                ModifiedByName = ((IReadOnlyNameable)modifiedBy).Name,
                ModifiedByTypeId = modifiedBy.SystemEntityType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = deletedBy.Id,
                DeletedByName = ((IReadOnlyNameable)deletedBy).Name,
                DeletedByTypeId = deletedBy.SystemEntityType.Id,
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };

            // act
            ModelExtensions.MapCreatedModifiedDeleted(item, entity);

            // assert
            item.ExtraProperty.Should().Be(Unchanged);
            item.Created.By.Id.Should().Be(createdBy.Id);
            item.Created.By.Name.Should().Be(createdBy.Name);
            item.Created.By.Type.Key.Should().Be(createdBy.SystemEntityType.Key);
            item.Created.By.Type.Name.Should().Be(createdBy.SystemEntityType.Name);
            item.Created.On.Should().Be(entity.CreatedOn);
            item.Modified.By.Id.Should().Be(modifiedBy.Id);
            item.Modified.By.Name.Should().Be(((IReadOnlyNameable)modifiedBy).Name);
            item.Modified.By.Type.Key.Should().Be(modifiedBy.SystemEntityType.Key);
            item.Modified.By.Type.Name.Should().Be(modifiedBy.SystemEntityType.Name);
            item.Modified.On.Should().Be(entity.ModifiedOn);
            item.Deleted.By.Id.Should().Be(deletedBy.Id);
            item.Deleted.By.Name.Should().Be(((IReadOnlyNameable)deletedBy).Name);
            item.Deleted.By.Type.Key.Should().Be(deletedBy.SystemEntityType.Key);
            item.Deleted.By.Type.Name.Should().Be(deletedBy.SystemEntityType.Name);
            item.Deleted.On.Should().Be(entity.DeletedOn);
        }
    }

    private class TestIdModel : Application.Common.Interfaces.IGuidId
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public Guid Id { get; set; }
    }

    private class TestCreatableModel : Application.Common.Interfaces.ICreatable
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
    }

    private class TestModifiableModel : Application.Common.Interfaces.IModifiable, Application.Common.Interfaces.ICreatable
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
        public RequiredOnByType Modified { get; set; } = new();
    }

    private class TestSoftDeletableModel : Application.Common.Interfaces.ISoftDeletable, Application.Common.Interfaces.IModifiable, Application.Common.Interfaces.ICreatable
    {
        public string ExtraProperty { get; set; } = Unchanged;
        public RequiredOnByType Created { get; set; } = new();
        public RequiredOnByType Modified { get; set; } = new();
        public OptionalOnByType Deleted { get; set; } = new();
    }

    private class TestIdEntity : IReadOnlyId
    {
        public Guid Id { get; set; }
    }

    private class TestCreatableEntity : ICreatable
    {
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    private class TestModifiableEntity : IModifiable, ICreatable
    {
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    private class TestSoftDeletableEntity : ISoftDeletable, IModifiable, ICreatable
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
    }
}
