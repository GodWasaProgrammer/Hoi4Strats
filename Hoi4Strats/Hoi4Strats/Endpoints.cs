using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedProj.DBModels;
using SharedProj.newsitems;

namespace Hoi4Strats;

public static class Endpoints
{
    private static readonly Dictionary<Guid, List<ImageModel>> TempImageStorage = [];

    public static void MapGuideEndpoints(this WebApplication app)
    {
        app.MapGet("/get-guides", async (DBService dbService) =>
        {
            var guides = await dbService.GetGuides();
            return Results.Ok(guides);
        });

        app.MapPost("/create-guide", async (GuideModel guide, [FromQuery] Guid? sessionId, DBService dbService) =>
        {
            if (sessionId.HasValue)
            {
                // Look up Images related to the guide
                if (TempImageStorage.TryGetValue(sessionId.Value, out var images))
                {
                    // add guides 
                    guide.Pictures = images;
                    TempImageStorage.Remove(sessionId.Value); // clear the tempstore
                }
                else
                {
                    return Results.BadRequest("SessionId not found or has no images.");
                }
            }

            // Skapa guiden
            await dbService.CreateGuide(guide);
            return Results.Ok("Guide created successfully");
        });

        app.MapPost("/Review-Decision", async (GuideModel guide, DBService dbService) =>
        {
            var res = await dbService.ReviewDecisionAsync(guide);

            if (res.Equals(guide))
            {
                return Results.Ok("successfully updated guide");
            }
            else
            {
                return Results.BadRequest("Something went wrong updating your decision");
            }
        });

        //app.MapPost("/create-guide", async (GuideModel guide, DBService dbService) =>
        //{
        //    await dbService.CreateGuide(guide);
        //    return Results.Ok("Guide created successfully");
        //});
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

            string? uploadSessionId;
            if (request.Headers.TryGetValue("Upload-Session-Id", out var headerValue))
            {
                uploadSessionId = headerValue;
            }
            else
            {
                // Hantera fallet när nyckeln saknas.
                throw new InvalidOperationException("Upload-Session-Id header is required.");
            }

            //string uploadSessionId = request.Headers["Upload-Session-Id"];

            if (file != null && !string.IsNullOrEmpty(uploadSessionId) && Guid.TryParse(uploadSessionId, out var sessionId))
            {
                // Lägg till bilden i mellanlagringen
                if (!TempImageStorage.ContainsKey(sessionId))
                {
                    TempImageStorage[sessionId] = [];
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
                    Url = base64Uri,
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
            if (json != null)
            {
                var news = JsonConvert.DeserializeObject<Root>(json);
                return news != null ? Results.Ok(news) : Results.BadRequest("Deserialization failed.");
            }
            else
            {
                return Results.BadRequest("There seems to be an issue with Steams API");
            }
        });
    }
}