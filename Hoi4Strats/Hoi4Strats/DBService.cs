using Microsoft.Data.SqlClient;
using Shared.DBModels;
using System.Data;

namespace Hoi4Strats;

public class DBService
{
    private readonly string _connectionString;

    public DBService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreateGuide(Guide GuideIn)
    {
        var query = "INSERT INTO Guides (Title, Content, Author, CreatedAt) VALUES (@Title, @Content, @Author, @CreatedAt)";

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = GuideIn.Title;
                    command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = GuideIn.Content;
                    command.Parameters.Add("@Author", SqlDbType.NVarChar).Value = GuideIn.Author;
                    command.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                    await command.ExecuteNonQueryAsync(); // Kör SQL-frågan
                }
            }
        }
        catch (Exception ex)
        {
            // Logga felet, exempelvis:
            Console.WriteLine($"An error occurred: {ex.Message}");
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

