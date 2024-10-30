using Hoi4Strats.Client.Data;
using System.Net.Http.Json;

namespace Hoi4Strats.Client.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ApplicationUser>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ApplicationUser>>("api/User/GetUsers");
    }

    public async Task<List<string>> GetRolesAsync(string userId)
    {
        return await _httpClient.GetFromJsonAsync<List<string>>($"api/User/GetRoles/{userId}");
    }

    public async Task UpdateRolesAsync(string userId, List<string> roles)
    {
        var dto = new UpdateRolesDto { UserId = userId, Roles = roles };
        var response = await _httpClient.PostAsJsonAsync("api/User/UpdateRoles", dto);
        response.EnsureSuccessStatusCode(); // Kasta ett undantag om svaret inte är framgångsrikt
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<string>>("api/User/GetAllRoles");
    }
}


public class UpdateRolesDto
{
    public string UserId { get; set; }
    public List<string> Roles { get; set; }
}
