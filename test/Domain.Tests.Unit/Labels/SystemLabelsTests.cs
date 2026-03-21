using AwesomeAssertions;
using Desic.Domain.Labels;

namespace Desic.Domain.Tests.Unit.Labels;

public class SystemLabelsTests
{
    public class SystemLabelsTests001 : SystemLabelsTests
    {
        [Fact]
        public void SystemEntityTypes_All_HaveUniqueValues()
        {
            var items = SystemLabels.All().ToList();
            var ids = items.Select(x => x.Id).ToList();
            var names = items.Select(x => x.Name).ToList();

            ids.Distinct().Count().Should().Be(ids.Count, because: "because all ids should be unique");
            names.Distinct(StringComparer.OrdinalIgnoreCase).Count().Should().Be(names.Count, because: "because all names should be unique");
        }
    }
}
