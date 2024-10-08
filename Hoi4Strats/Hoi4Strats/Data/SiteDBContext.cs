using System.Data.SqlClient;

namespace Hoi4Strats.Data
{
    public class DBService
    {
        private readonly string _connectionString;

        public DBService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateGuide(string title, string content, string author)
        {
            var query = "INSERT INTO Guides (Title, Content, Author, CreatedAt) VALUES (@Title, @Content, @Author, @CreatedAt)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@Author", author);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

