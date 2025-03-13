using Microsoft.Data.SqlClient;
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

    //public async Task CreateGuide(GuideModel GuideIn)
    //{
    //    var query = "INSERT INTO Guides (Title, Content, Author, CreatedAt) VALUES (@Title, @Content, @Author, @CreatedAt)";

    //    try
    //    {
    //        using (var connection = new SqlConnection(_connectionString))
    //        {
    //            await connection.OpenAsync();

    //            using (var command = new SqlCommand(query, connection))
    //            {
    //                command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = GuideIn.Title;
    //                command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = GuideIn.Content;
    //                command.Parameters.Add("@Author", SqlDbType.NVarChar).Value = GuideIn.Author;
    //                command.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

    //                await command.ExecuteNonQueryAsync(); // Kör SQL-frågan
    //                Console.WriteLine($"{GuideIn.Title} written to DB");
    //                Console.WriteLine($"{GuideIn.Content} written to DB");
    //                Console.WriteLine($"{GuideIn.Author} written to DB");
    //                Console.WriteLine($"{GuideIn.CreatedAt} written to DB");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Logga felet, exempelvis:
    //        Console.WriteLine($"An error occurred: {ex.Message}");
    //    }
    //}
    public async Task CreateGuide(GuideModel guideIn)
    {
        var guideQuery = "INSERT INTO Guides (Title, Content, Author, CreatedAt) OUTPUT INSERTED.Id VALUES (@Title, @Content, @Author, @CreatedAt)";
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
                            guideCommand.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

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
                                    imageCommand.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;
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
        var query = "SELECT Id, Title, Content, Author, CreatedAt FROM Guides";

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
                        CreatedAt = reader.GetDateTime(4)
                    };
                    guides.Add(guide);
                }
            }
        }
        return guides;
    }

    internal async Task CreateTemplateGuide(GuideModel guide)
    {
        var query = "INSERT INTO Guides (Title, Content, Author, CreatedAt) VALUES (@Title, @Content, @Author, @CreatedAt)";

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = guide.Title;
                    command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = guide.Content;
                    command.Parameters.Add("@Author", SqlDbType.NVarChar).Value = guide.Author;
                    command.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                    await command.ExecuteNonQueryAsync(); // Kör SQL-frågan
                    Console.WriteLine($"{guide.Title} written to DB");
                    Console.WriteLine($"{guide.Content} written to DB");
                    Console.WriteLine($"{guide.Author} written to DB");
                    Console.WriteLine($"{guide.CreatedAt} written to DB");
                }
            }
        }
        catch (Exception ex)
        {
            // Logga felet, exempelvis:
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}