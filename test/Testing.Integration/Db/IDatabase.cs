namespace Desic.Testing.Integration.Db;

public interface IDatabase : IDatabaseServer
{
    string DatabaseName { get; }
}
