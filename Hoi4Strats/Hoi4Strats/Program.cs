using Hoi4Strats.Client.Services;
using Hoi4Strats.Components;
using Hoi4Strats.Components.Account;
using Hoi4Strats.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Radzen;
using Shared.DBModels;
using Shared.newsitems;
using System.Net;

namespace Hoi4Strats;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        // Add services to the container
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
        var dbServiceString = DButils.GetConnectionString();

        builder.Services.AddScoped<DBService>(provider => new DBService(dbServiceString));
        builder.Services.AddScoped<UserService>();
        builder.Services.AddRadzenComponents().AddScoped<ThemeService>().AddScoped<NotificationService>();
        builder.Services.AddRadzenCookieThemeService(options =>
        {
            options.Name = "MyApplicationTheme";
            options.Duration = TimeSpan.FromDays(365);
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Identity configuration with roles
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7141/") });

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<DialogService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .AddInteractiveWebAssemblyRenderMode()
           .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.MapAdditionalIdentityEndpoints();
        app.UseRouting();
        app.UseAntiforgery();
        app.MapControllers();

        // Custom endpoints
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

        app.MapPost("/get-hoi4-news", async () =>
        {
            var steamapi = new SteamApiClient();
            var json = await steamapi.GetNewsForHeartsOfIronAsync();
            var news = JsonConvert.DeserializeObject<Root>(json);
            return news != null ? Results.Ok(news) : Results.BadRequest("Deserialization failed.");
        });

        Tests.TestConnection();

        // Ensure roles are created when application starts
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await EnsureRolesCreated(roleManager);
        }

        await app.RunAsync();
    }

    private static async Task EnsureRolesCreated(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User", "Editor", "Moderator", "GuideAdmin", "ForumAdmin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}

