using Newtonsoft.Json;
using SharedProj.DBModels;
using SharedProj.newsitems;

namespace Hoi4Strats
{
    // Extensions/EndpointExtensions.cs
    public static class Endpoints
    {
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

}
