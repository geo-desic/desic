using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Common.Models;

public class CreatableModelTests
{
    public class CreatableModelTests001 : CreatableModelTests
    {
        [Fact]
        public void Contructor_WithCreatableEntity_AllPropertiesMappedCorrectly()
        {
            // arrange
            var createdByType = SystemEntityTypes.Unspecified;
            var entity = new TestEntity
            {
                Id = 1.ToGuid(),
                CreatedById = 2.ToGuid(),
                CreatedByName = nameof(TestEntity.CreatedByName),
                CreatedByTypeId = createdByType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
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
            };

            // act
            var result = new TestModel(entity);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    private class TestEntity : IReadOnlyCreatableEntity
    {
        public Guid Id { get; init; }
        public Guid CreatedById { get; init; }
        public string? CreatedByName { get; init; }
        public Guid CreatedByTypeId { get; init; }
        public DateTime CreatedOn { get; init; }
        public SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }

    private class TestModel : CreatableModel
    {
        public TestModel() : base() { }
        public TestModel(IReadOnlyCreatableEntity from) : base(from) { } // we are testing this base(from) call to make sure it sets all expected properties
    }
}
