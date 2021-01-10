using Desic.Entities;
using System.Threading.Tasks;

namespace Desic.Repositories
{
    public interface IUsersRepository
    {
        Task<IUser> Get(long id);
    }
}
