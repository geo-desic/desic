namespace Desic.Api.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async void Get_UserDoesNotExist_Status404NotFound()
        {
        }

        [Fact]
        public async void Get_UserExistsButNotAuthorizedToAccess_Status403Forbidden()
        {
        }

        [Fact]
        public async void Get_UserExistsAndAuthorizedToAccess_Status200OK()
        {
        }
    }
}
