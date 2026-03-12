namespace Desic.Infrastructure.Data.SqlServer;

public static class ConfigKeys
{
    public const string InitializationEnabled = $"{SectionInitialization}:Enabled";
    public const string SectionInitialization = $"{SectionSqlServer}:Initialization";
    public const string SectionSqlServer = "Databases:Application:SqlServer";
}
