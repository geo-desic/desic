namespace Desic.Testing.Integration.Db;

internal static class SqliteOperations
{
    public static void DeleteDatabaseAndAssociatedFiles(string databaseFilePath)
    {
        // delete the primary database file
        if (databaseFilePath != null) File.Delete(databaseFilePath);

        // delete any associated files that sqlite automatically creates
        string[] associatedExtensions = ["db-journal", "db-wal", "db-shm"];
        foreach (var extension in associatedExtensions)
        {
            var filepath = Path.ChangeExtension(databaseFilePath, extension);
            if (File.Exists(filepath)) File.Delete(filepath);
        }
    }
}
