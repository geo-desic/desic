using AwesomeAssertions;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tests.Unit.EntityTypes;

public class SystemEntityTypesTests
{
    public class SystemEntityTypesTests001 : SystemEntityTypesTests
    {
        [Fact]
        public void SystemEntityTypes_All_HaveUniqueValues()
        {
            var items = SystemEntityTypes.All().ToList();
            var ids = items.Select(x => x.Id).ToList();
            var keys = items.Select(x => x.Key).ToList();
            var names = items.Select(x => x.Name).ToList();

            ids.Distinct().Count().Should().Be(ids.Count, because: "because all ids should be unique");
            keys.Distinct(StringComparer.OrdinalIgnoreCase).Count().Should().Be(keys.Count, because: "because all keys should be unique");
            names.Distinct(StringComparer.OrdinalIgnoreCase).Count().Should().Be(names.Count, because: "because all names should be unique");
        }
    }

    public class SystemEntityTypesTests002 : SystemEntityTypesTests
    {
        [Fact]
        public void SystemEntityTypes_All_PassValidation()
        {
            var systemItems = SystemEntityTypes.All().ToList();
            var items = SystemEntityTypes.AllAsEntities().ToList();

            foreach (var item in systemItems)
            {
                new SystemEntityTypeValidator().Validate(item).IsValid.Should().BeTrue(because: "because '{0}' should pass validation", becauseArgs: item.Name);
            }

            foreach (var item in items)
            {
                new EntityTypeValidator().Validate(item).IsValid.Should().BeTrue(because: "because '{0}' should pass validation", becauseArgs: item.Name);
            }
        }
    }
}
