using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Domain.Common.Interfaces;
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

    private class TestEntity : IReadOnlyGuidId
    {
        public Guid Id { get; init; }
    }

    private class TestModel : BaseModel
    {
        public TestModel() : base() { }
        public TestModel(IReadOnlyGuidId from) : base(from) { } // we are testing this base(from) call to make sure it sets all expected properties
    }
}
