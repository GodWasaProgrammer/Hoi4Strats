using Newtonsoft.Json;
using SharedProj.DBModels;
using SharedProj.newsitems;

namespace Hoi4Strats;

public static class Endpoints
{
    private static readonly Dictionary<Guid, List<ImageModel>> TempImageStorage = new();

    public static void MapGuideEndpoints(this WebApplication app)
    {
        app.MapGet("/get-guides", async (DBService dbService) =>
        {
            var guides = await dbService.GetGuides();
            return Results.Ok(guides);
        });

        app.MapPost("/create-guide", async (GuideModel guide, DBService dbService) =>
        {
            await dbService.CreateGuide(guide);
            return Results.Ok("Guide created successfully");
        });
    }

    public static void MapImageUploadEndpoint(this WebApplication app)
    {
        app.MapPost("/upload/image", async (HttpRequest request) =>
        {
            var form = await request.ReadFormAsync();
            IFormFile? file = form.Files.FirstOrDefault();
            if (file != null)
            {
                var filePath = Path.Combine("wwwroot", "uploads", file.FileName);
                Directory.CreateDirectory(Path.Combine("wwwroot", "uploads"));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var fileUrl = $"/uploads/{file.FileName}";
                return Results.Ok(new { Url = fileUrl });
            }
            return Results.BadRequest("Ingen fil uppladdad.");
        });
    }

    public static void BlobImageUpload(this WebApplication app)
    {
        app.MapPost("/upload/blob", async (HttpRequest request) =>
        {
            var form = await request.ReadFormAsync();
            IFormFile? file = form.Files.FirstOrDefault();
            string uploadSessionId = request.Headers["Upload-Session-Id"];

            if (file != null && !string.IsNullOrEmpty(uploadSessionId) && Guid.TryParse(uploadSessionId, out var sessionId))
            {
                // Lägg till bilden i mellanlagringen
                if (!TempImageStorage.ContainsKey(sessionId))
                {
                    TempImageStorage[sessionId] = new List<ImageModel>();
                }

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var image = new ImageModel
                {
                    Name = file.FileName,
                    Content = ms.ToArray(),
                    ContentType = file.ContentType,
                    CreatedAt = DateTime.UtcNow
                };

                TempImageStorage[sessionId].Add(image);

                // Konvertera bilden till en Base64-data-URI
                var base64Uri = ToBase64DataUri(image);

                return Results.Ok(new
                {
                    Message = "Image uploaded successfully",
                    SessionId = sessionId,
                    Base64Uri = base64Uri,
                    ImageName = image.Name
                });
            }

            return Results.BadRequest("Invalid upload session or no file uploaded.");
        });
    }

    public static string ToBase64DataUri(ImageModel img)
    {
        return $"data:{img.ContentType};base64,{Convert.ToBase64String(img.Content)}";
    }

    public static void MapNewsEndpoint(this WebApplication app)
    {
        app.MapPost("/get-hoi4-news", async () =>
        {
            var steamapi = new SteamApiClient();
            var json = await steamapi.GetNewsForHeartsOfIronAsync();
            var news = JsonConvert.DeserializeObject<Root>(json);
            return news != null ? Results.Ok(news) : Results.BadRequest("Deserialization failed.");
        });
    }
}