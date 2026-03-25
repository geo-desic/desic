using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Common.Models;

public class ModifiableModelTests
{
    public class ModifiableModelTests001 : ModifiableModelTests
    {
        [Fact]
        public void Contructor_WithModifiableEntity_AllPropertiesMappedCorrectly()
        {
            // arrange
            var createdByType = SystemEntityTypes.Unspecified;
            var modifiedByType = SystemEntityTypes.Label;
            var entity = new TestEntity
            {
                Id = 1.ToGuid(),
                CreatedById = 2.ToGuid(),
                CreatedByName = "CreatedByName",
                CreatedByTypeId = createdByType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 3.ToGuid(),
                ModifiedByName = "ModifiedByName",
                ModifiedByTypeId = modifiedByType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
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
            };

            // act
            var result = new TestModel(entity);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    private class TestEntity : ModifiableEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }

    private class TestModel : ModifiableModel
    {
        public TestModel() : base() { }
        public TestModel(ModifiableEntity entity) : base(entity) { } // we are testing this base(entity) call to make sure it sets all expected properties
    }
}
