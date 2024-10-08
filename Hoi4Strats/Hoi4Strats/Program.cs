using Hoi4Strats.Components;
using Hoi4Strats.Components.Account;
using Hoi4Strats.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Shared.DBModels;

namespace Hoi4Strats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

            var dbServiceString = DButils.GetConnectionString();

            // Registrera DBService med anslutningssträngen
            builder.Services.AddScoped<DBService>(provider => new DBService(dbServiceString));
            builder.Services.AddRadzenComponents().AddScoped<ThemeService>().AddScoped<NotificationService>(); // För att hantera notifieringar
            builder.Services.AddRadzenCookieThemeService(options =>
            {
                options.Name = "MyApplicationTheme";
                options.Duration = TimeSpan.FromDays(365);
            });
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5215/") });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode()
               .AddInteractiveWebAssemblyRenderMode()
               .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.

            app.MapAdditionalIdentityEndpoints();

            app.MapGet("/get-guides", async (DBService dbService) =>
            {
                var guides = await dbService.GetGuides();
                return Results.Ok(guides);
            });

            app.MapPost("/create-guide", async (Guide guide, DBService dbService) =>
            {
                await dbService.CreateGuide(guide);
                return Results.Ok("Guide created successfully");
            });

            Tests.TestConnection();
            app.Run();

        }
    }
}
