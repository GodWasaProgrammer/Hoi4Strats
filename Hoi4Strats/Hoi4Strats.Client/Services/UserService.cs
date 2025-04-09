using SharedProj;
using System.Net.Http.Json;

namespace Hoi4Strats.Client.Services;

public class UserService
{
    private readonly AuthenticatedHttpClient _httpClient;

    public UserService(AuthenticatedHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ApplicationUser>> GetUsersAsync()
    {
        var result = await _httpClient.Client.GetFromJsonAsync<List<ApplicationUser>>("api/User/GetUsers");

        if (result == null)
        {
            throw new HttpRequestException("Failed to retrieve users");
        }
        else
        {
            return result;
        }
    }

    public async Task<List<string>> GetRolesAsync(string userId)
    {
        var result = await _httpClient.Client.GetFromJsonAsync<List<string>>($"api/User/GetRoles/{userId}");

        if (result == null)
        {
            throw new HttpRequestException($"Failed to fetch roles for user{userId}");
        }
        else
        {
            return result;
        }
    }

    public async Task<string> UpdateRolesAsync(string userId, List<string> roles)
    {
        var dto = new UpdateRolesDto { UserId = userId, Roles = roles };
        var response = await _httpClient.Client.PostAsJsonAsync("api/User/UpdateRoles", dto);
        response.EnsureSuccessStatusCode(); // Kasta ett undantag om svaret inte är framgångsrikt

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            if (content.Contains("<"))
            {
                return "Access Denied";
            }
            Console.WriteLine($"HTTP Response:{content}");

        }
        if (response.Headers.Location != null)
        {
            Console.WriteLine($"{response.Headers.Location}");
        }

        if (response.StatusCode.ToString() == "NoContent")
        {
            return "OK";
        }
        else
        {
            return response.StatusCode.ToString();
        }
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        var result = await _httpClient.Client.GetFromJsonAsync<List<string>>("api/User/GetAllRoles");

        if (result == null)
        {
            throw new HttpRequestException("Failed to retrieve all possible roles");
        }
        else
        {
            return result;
        }
    }
}