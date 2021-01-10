using Desic.Entities;
using Desic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesicTests.Desic
{
    public class FakeUsersRepository : IUsersRepository
    {
        private static readonly List<User> _users;
        static FakeUsersRepository()
        {
            var i = 0;
            _users = new List<User>()
            {
                new User() { Id = Guid.NewGuid(), SequentialId = ++i, Username = "admin", Hidden = false, CreatedOn = DateTime.UtcNow, CreatedBy = "system", CreatedByType = "system", ModifiedOn = DateTime.UtcNow, ModifiedBy = "system", ModifiedByType = "system" },
                new User() { Id = Guid.NewGuid(), SequentialId = ++i, Username = "bschultz", Hidden = false, CreatedOn = DateTime.UtcNow, CreatedBy = "system", CreatedByType = "system", ModifiedOn = DateTime.UtcNow, ModifiedBy = "system", ModifiedByType = "system" },
            };
        }

        public async Task<IUser> Get(long id)
        {
            return await Task.Run(() => _users.Where(x => x.SequentialId == id).FirstOrDefault());
        }
    }
}
