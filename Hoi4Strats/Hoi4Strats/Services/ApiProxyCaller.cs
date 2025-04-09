using SharedProj;

namespace Hoi4Strats.Services;


public class ApiProxyCaller : IUserService
{
    public ApiProxyCaller()
    {

    }
    async Task<List<string>> IUserService.GetAllRolesAsync()
    {
        throw new NotImplementedException();
    }

    async Task<List<string>> IUserService.GetRolesAsync(string userId)
    {
        throw new NotImplementedException();
    }

    async Task<List<ApplicationUser>> IUserService.GetUsersAsync()
    {
        throw new NotImplementedException();
    }

    async Task<string> IUserService.UpdateRolesAsync(string userId, List<string> roles)
    {
        throw new NotImplementedException();
    }
}
