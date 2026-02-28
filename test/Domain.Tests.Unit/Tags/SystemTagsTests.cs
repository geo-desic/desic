using AwesomeAssertions;
using Desic.Domain.Tags;

namespace Desic.Domain.Tests.Unit.Tags;

public class SystemTagsTests
{
    [Fact]
    public void SystemEntityTypes_All_HaveUniqueValues()
    {
        var items = SystemTags.All().ToList();
        var ids = items.Select(x => x.Id).ToList();
        var names = items.Select(x => x.Name).ToList();

        ids.Distinct().Count().Should().Be(ids.Count, because: "because all ids should be unique");
        names.Distinct(StringComparer.OrdinalIgnoreCase).Count().Should().Be(names.Count, because: "because all names should be unique");
    }
}
