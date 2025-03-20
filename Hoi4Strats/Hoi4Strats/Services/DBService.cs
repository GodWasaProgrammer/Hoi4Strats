using Microsoft.Data.SqlClient;
using SharedProj;
using SharedProj.DBModels;
using System.Data;

namespace Hoi4Strats;

public class DBService
{
    private readonly string _connectionString;

    public DBService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreateGuide(GuideModel guideIn)
    {
        var guideQuery = "INSERT INTO Guides (Title, Content, Author, CreatedAt, GuideType) OUTPUT INSERTED.Id VALUES (@Title, @Content, @Author, @CreatedAt, @GuideType)";
        var imageQuery = "INSERT INTO Images (Name, Content, ContentType, CreatedAt, GuideId) VALUES (@Name, @Content, @ContentType, @CreatedAt, @GuideId)";

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // insert guide and fetch guid
                        int guideId;
                        using (var guideCommand = new SqlCommand(guideQuery, connection, transaction))
                        {
                            guideCommand.Parameters.Add("@Title", SqlDbType.NVarChar).Value = guideIn.Title;
                            guideCommand.Parameters.Add("@Content", SqlDbType.NVarChar).Value = guideIn.Content;
                            guideCommand.Parameters.Add("@Author", SqlDbType.NVarChar).Value = guideIn.Author;
                            guideCommand.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.UtcNow;
                            guideCommand.Parameters.Add("@GuideType", SqlDbType.Int).Value = (int)guideIn.GuideType; // Konvertera enum till int

                            guideId = (int)await guideCommand.ExecuteScalarAsync();
                        }

                        // insert pictures
                        if (guideIn.Pictures != null && guideIn.Pictures.Any())
                        {
                            foreach (var image in guideIn.Pictures)
                            {
                                using (var imageCommand = new SqlCommand(imageQuery, connection, transaction))
                                {
                                    imageCommand.Parameters.Add("@Name", SqlDbType.NVarChar).Value = image.Name;
                                    imageCommand.Parameters.Add("@Content", SqlDbType.VarBinary).Value = image.Content;
                                    imageCommand.Parameters.Add("@ContentType", SqlDbType.NVarChar).Value = image.ContentType;
                                    imageCommand.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.UtcNow;
                                    imageCommand.Parameters.Add("@GuideId", SqlDbType.Int).Value = guideId;

                                    await imageCommand.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        // Commit 
                        await transaction.CommitAsync();
                        Console.WriteLine($"Guide '{guideIn.Title}' and its images have been written to the DB.");
                    }
                    catch
                    {
                        // Rollback on error
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async Task<List<GuideModel>> GetGuides()
    {
        var guides = new List<GuideModel>();
        var query = "SELECT Id, Title, Content, Author, CreatedAt, GuideType FROM Guides";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var guide = new GuideModel
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Content = reader.GetString(2),
                        Author = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                        GuideType = (GuideTypes)reader.GetInt32(5) // Konvertera int till enum
                    };
                    guide.Pictures = await GetPicturesForGuide(guide.Id);
                    guides.Add(guide);
                }
            }
        }
        return guides;
    }

    private async Task<List<ImageModel>> GetPicturesForGuide(int guideId)
    {
        var pictures = new List<ImageModel>();
        var query = "SELECT ImageId, Name, Content, ContentType, CreatedAt FROM Images WHERE GuideId = @GuideId";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@GuideId", guideId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var picture = new ImageModel
                        {
                            ImageId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Content = (byte[])reader["Content"],
                            ContentType = reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4),
                            GuideId = guideId
                        };
                        pictures.Add(picture);
                    }
                }
            }
        }
        return pictures;
    }
}