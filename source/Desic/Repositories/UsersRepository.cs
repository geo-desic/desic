using Dapper;
using Desic.Data;
using Desic.Entities;
using System.Threading.Tasks;

namespace Desic.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsersRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IUser> Get(long id)
        {
            using (var connection = _connectionFactory.Create())
            {
                return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM [act].[Users] WHERE [SequentialId] = @Id", new { Id = id });
            }
        }
    }
}
