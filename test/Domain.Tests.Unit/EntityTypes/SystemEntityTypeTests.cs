using AwesomeAssertions;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tests.Unit.EntityTypes;

public class SystemEntityTypeTests
{
    public class SystemEntityTypeTests001 : SystemEntityTypeTests
    {
        public static IEnumerable<TheoryDataRow<bool, SystemEntityType?, SystemEntityType?>> IsEquivalentToTheoryData()
        {
            SystemEntityType? item1, item2;

            // true: all fields equal
            item1 = NewItem();
            item2 = NewItem();
            yield return new(true, item1, item2);

            // true: both null
            item1 = null;
            item2 = null;
            yield return new(true, item1, item2);

            // false: only item1 null
            item1 = null;
            item2 = NewItem();
            yield return new(false, item1, item2);

            // false: only item2 null
            item1 = NewItem();
            item2 = null;
            yield return new(false, item1, item2);

            // false: property does not match: Key
            item1 = NewItem(key: "key1");
            item2 = NewItem(key: "key2");
            yield return new(false, item1, item2);

            // false: property does not match: Name
            item1 = NewItem(name: "Name1");
            item2 = NewItem(name: "Name2");
            yield return new(false, item1, item2);
        }

        [Theory]
        [MemberData(nameof(IsEquivalentToTheoryData))]
        public void IsEquivalentTo_SpecifiedTheoryData_ExpectedResult(bool expected, SystemEntityType? item1, SystemEntityType? item2)
        {
            item1.IsEquivalentTo(item2).Should().Be(expected);
        }
    }

    private static SystemEntityType NewItem(string key = "key", string name = "Name")
    {
        return new SystemEntityType(Id: Guid.CreateVersion7(), Key: key, Name: name);
    }
}
