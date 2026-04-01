using AwesomeAssertions;
using Desic.Domain.EntityTypes;
using Xunit.Sdk;

namespace Desic.Domain.Tests.Unit.EntityTypes;

public class EntityTypeExtensionsTests
{
    public class EntityTypeExtensionsTests001 : EntityTypeExtensionsTests
    {
        public static IEnumerable<TheoryDataRow<bool, TestItem?, TestItem?>> IsEquivalentToTheoryData()
        {
            TestItem? item1, item2;

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
        public void IsEquivalentTo_SpecifiedTheoryData_ExpectedResult(bool expected, TestItem? item1, TestItem? item2)
        {
            EntityTypeExtensions.IsEquivalentTo(item1, item2).Should().Be(expected);
        }
    }

    private static TestItem NewItem(string key = "key", string name = "Name")
    {
        return new TestItem { Key = key, Name = name };
    }

    public sealed class TestItem : IReadOnlyEntityType, IXunitSerializable
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public void Deserialize(IXunitSerializationInfo info)
        {
            Key = info.GetValue<string>(nameof(Key))!;
            Name = info.GetValue<string>(nameof(Name))!;
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(Name), Name);
        }
    }
}
