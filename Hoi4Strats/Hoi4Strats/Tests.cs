using Hoi4Strats.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace Hoi4Strats
{
    public class Tests
    {
        public static void TestConnection()
        {
            var connectionString = DButils.GetConnectionString();

            using var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("SQL Connection test Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Houston, we got a SQL problem: {ex.Message}");
            }
        }

        public static async Task EnsureRolesCreated(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "User", "Editor", "Moderator", "GuideAdmin", "ForumAdmin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
