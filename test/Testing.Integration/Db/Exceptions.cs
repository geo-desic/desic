namespace Desic.Testing.Integration.Db;

internal static class Exceptions
{
    public static InvalidOperationException DatabaseNotInitialized() => new("Database has not been initialized");
}
