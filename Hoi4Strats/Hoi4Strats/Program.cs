using Hoi4Strats.Client.Services;
using Hoi4Strats.Components;
using Hoi4Strats.Components.Account;
using Hoi4Strats.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SharedProj;
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

        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        //    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(dbServiceString));
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
        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .AddInteractiveWebAssemblyRenderMode()
           .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.MapAdditionalIdentityEndpoints();
        app.UseRouting();
        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        Endpoints.MapGuideEndpoints(app);
        Endpoints.MapNewsEndpoint(app);
        Endpoints.MapImageUploadEndpoint(app);
        Tests.TestConnection();

        // Ensure roles are created when application starts
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await Tests.EnsureRolesCreated(roleManager);
        }

        await app.RunAsync();
    }
}