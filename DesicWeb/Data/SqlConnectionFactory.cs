using Desic.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DesicWeb.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
        public IDbConnection CreateAndOpen()
        {
            var connection = Create();
            connection.Open();
            return connection;
        }
    }
}
