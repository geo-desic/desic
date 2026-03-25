using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Entities;
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
                CreatedByName = "CreatedByName",
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

    private class TestEntity : CreatableEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }

    private class TestModel : CreatableModel
    {
        public TestModel() : base() { }
        public TestModel(CreatableEntity entity) : base(entity) { } // we are testing this base(entity) call to make sure it sets all expected properties
    }
}
