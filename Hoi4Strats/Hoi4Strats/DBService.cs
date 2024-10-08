using Hoi4Strats.DBModels;
using System.Data.SqlClient;

namespace Hoi4Strats
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

                    await command.ExecuteNonQueryAsync(); // Kör SQL-frågan
                }
            }
        }

        public async Task<List<Guide>> GetGuides()
        {
            var guides = new List<Guide>();
            var query = "SELECT Id, Title, Content, Author, CreatedAt FROM Guides";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var guide = new Guide
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Content = reader.GetString(2),
                            Author = reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4)
                        };
                        guides.Add(guide);
                    }
                }
            }
            return guides;
        }
    }
}

