using SharedProj;

namespace SharedProj;

public interface IUserService
{
    Task<List<ApplicationUser>> GetUsersAsync();
    Task<List<string>> GetRolesAsync(string userId);
    Task<string> UpdateRolesAsync(string userId, List<string> roles);
    Task<List<string>> GetAllRolesAsync();
}
