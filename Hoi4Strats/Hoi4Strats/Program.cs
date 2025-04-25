using FindRazorSourceFile.Server;
using Hoi4Strats.Client.Services;
using Hoi4Strats.Components;
using Hoi4Strats.Components.Account;
using Hoi4Strats.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using SharedProj;
using System.Text;

namespace Hoi4Strats;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: builder.Configuration["Jwt:Key"]))
                };
            });
        builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        });// Add services to the container
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
            options.IsSecure = true;
            options.SameSite = CookieSameSiteMode.Lax;
        });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(dbServiceString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Identity configuration with roles
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()

            // Tokens
            .AddDefaultTokenProviders();
        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax; // Eller Strict beroende på behov
            options.Secure = CookieSecurePolicy.Always;       // Kräver HTTPS
        });

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        // .NET 9 setup of TLS 1.2
        builder.Services.AddScoped(sp =>
        {
            var handler = new SocketsHttpHandler
            {
                UseCookies = true,
                Credentials = System.Net.CredentialCache.DefaultCredentials,
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                    RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
                }
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7141/")
            };
        });
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        })
        .AddJsonProtocol();
        var app = builder.Build();
        app.UseCors(policy =>
        {
            policy.WithOrigins("https://localhost:7141")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Krävs för cookies och WebSockets
        });

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
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .AddInteractiveWebAssemblyRenderMode()
           .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.MapAdditionalIdentityEndpoints();

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            Secure = CookieSecurePolicy.Always, // Kräver HTTPS för alla cookies
            MinimumSameSitePolicy = SameSiteMode.Lax // Eller SameSiteMode.Strict beroende på krav
        });
        app.UseRouting();
        app.UseAntiforgery();

        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGuideEndpoints();
        app.MapNewsEndpoint();
        app.MapImageUploadEndpoint();
        app.BlobImageUpload();
        Tests.TestConnection();
        app.UseFindRazorSourceFile();

        // Ensure roles are created when application starts
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await Tests.EnsureRolesCreated(roleManager);
        }

        await app.RunAsync();
    }
}