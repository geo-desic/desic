using CsvHelper.Configuration;

namespace Desic.Infrastructure.Tests.Unit.Data.Resources;

public class TestResourceClassMap : ClassMap<TestResource>
{
    public TestResourceClassMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Name).Name("name");
        Map(m => m.Value).Name("value");
    }
}
