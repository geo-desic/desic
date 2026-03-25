using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Common.Models;

public class BaseModelTests
{
    public class BaseModelTests001 : BaseModelTests
    {
        [Fact]
        public void Contructor_WithBaseEntity_AllPropertiesMappedCorrectly()
        {
            // arrange
            var entity = new TestEntity
            {
                Id = 1.ToGuid(),
            };
            var expected = new TestModel
            {
                Id = entity.Id,
            };

            // act
            var result = new TestModel(entity);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    private class TestEntity : BaseEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
    }

    private class TestModel : BaseModel
    {
        public TestModel() : base() { }
        public TestModel(BaseEntity entity) : base(entity) { } // we are testing this base(entity) call to make sure it sets all expected properties
    }
}
