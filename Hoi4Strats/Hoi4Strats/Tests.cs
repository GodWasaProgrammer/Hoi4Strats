using Microsoft.Data.SqlClient;

namespace Hoi4Strats
{
    public class Tests
    {
        public void TestConnection()
        {
            var connectionString = "Server=DESKTOP-AJ1T1NR;Database=SiteDB;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection Success!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Houston, we got a SQL problem: {ex.Message}");
                }
            }
        }
    }
}
