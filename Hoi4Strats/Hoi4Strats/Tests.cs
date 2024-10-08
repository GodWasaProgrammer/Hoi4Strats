using Hoi4Strats.Data;
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
    }
}
