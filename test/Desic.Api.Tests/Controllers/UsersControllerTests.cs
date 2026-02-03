using System.Threading.Tasks;

namespace Desic.Api.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task Get_UserDoesNotExist_Status404NotFound()
        {
        }

        [Fact]
        public async Task Get_UserExistsButNotAuthorizedToAccess_Status403Forbidden()
        {
        }

        [Fact]
        public async Task Get_UserExistsAndAuthorizedToAccess_Status200OK()
        {
        }
    }
}
