using AwesomeAssertions;
using Desic.Domain.Users.Test;

namespace Desic.Domain.Tests.Unit.Users.Test;

public class TestUsersTests
{
    public class TestUsersTests001 : TestUsersTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Generate_SpecifiedCount_ExpectedResults(int count)
        {
            // act
            var results = await TestUsers.Generate(count: count, cancellationToken: TestContext.Current.CancellationToken).ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

            // assert
            var stop = count < TestUsers.PredefinedCount ? count : TestUsers.PredefinedCount;
            for (var i = 1; i <= stop; ++i)
            {
                results[i - 1].Should().BeEquivalentTo(TestUsers.GetPredefined(sequentialId: i));
            }
            results.Count.Should().Be(count);
            for (var i = TestUsers.PredefinedCount; i <= count; ++i)
            {
                results[i - 1].Should().NotBeNull();
            }
        }
    }
}
