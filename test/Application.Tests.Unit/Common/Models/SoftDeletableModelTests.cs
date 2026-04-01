using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Common.Models;

public class SoftDeletableModelTests
{
    public class SoftDeletableModelTests001 : SoftDeletableModelTests
    {
        [Fact]
        public void Contructor_WithSoftDeletableEntity_AllPropertiesMappedCorrectly()
        {
            // arrange
            var createdByType = SystemEntityTypes.Unspecified;
            var modifiedByType = SystemEntityTypes.Label;
            var deletedByType = SystemEntityTypes.User;
            var entity = new TestEntity
            {
                Id = 1.ToGuid(),
                CreatedById = 2.ToGuid(),
                CreatedByName = nameof(TestEntity.CreatedByName),
                CreatedByTypeId = createdByType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 3.ToGuid(),
                ModifiedByName = nameof(TestEntity.ModifiedByName),
                ModifiedByTypeId = modifiedByType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 4.ToGuid(),
                DeletedByName = nameof(TestEntity.DeletedByName),
                DeletedByTypeId = deletedByType.Id,
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            };
            var expected = new TestModel
            {
                Id = entity.Id,
                Created = new()
                {
                    By = new()
                    {
                        Id = entity.CreatedById,
                        Name = entity.CreatedByName,
                        Type = new()
                        {
                            Key = createdByType.Key,
                            Name = createdByType.Name,
                        },
                    },
                    On = entity.CreatedOn,
                },
                Modified = new()
                {
                    By = new()
                    {
                        Id = entity.ModifiedById,
                        Name = entity.ModifiedByName,
                        Type = new()
                        {
                            Key = modifiedByType.Key,
                            Name = modifiedByType.Name,
                        },
                    },
                    On = entity.ModifiedOn,
                },
                Deleted = new()
                {
                    By = new()
                    {
                        Id = entity.DeletedById,
                        Name = entity.DeletedByName,
                        Type = new()
                        {
                            Key = deletedByType.Key,
                            Name = deletedByType.Name,
                        },
                    },
                    On = entity.DeletedOn,
                },
            };

            // act
            var result = new TestModel(entity);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    private class TestEntity : IReadOnlySoftDeletableEntity
    {
        public Guid Id { get; init; }
        public Guid CreatedById { get; init; }
        public string? CreatedByName { get; init; }
        public Guid CreatedByTypeId { get; init; }
        public DateTime CreatedOn { get; init; }
        public Guid ModifiedById { get; init; }
        public string? ModifiedByName { get; init; }
        public Guid ModifiedByTypeId { get; init; }
        public DateTime ModifiedOn { get; init; }
        public Guid? DeletedById { get; init; }
        public string? DeletedByName { get; init; }
        public Guid? DeletedByTypeId { get; init; }
        public DateTime? DeletedOn { get; init; }
        public bool IsDeleted { get => DeletedOn.HasValue; }
        public SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }

    private class TestModel : SoftDeletableModel
    {
        public TestModel() : base() { }
        public TestModel(IReadOnlySoftDeletableEntity from) : base(from) { } // we are testing this base(from) call to make sure it sets all expected properties
    }
}
