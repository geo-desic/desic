using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

public interface ISqlServer : IDatabaseServer
{
    SqlConnection GetSqlServerConnection();
    DbConnection IDatabaseServer.GetConnection() => GetSqlServerConnection();
}
