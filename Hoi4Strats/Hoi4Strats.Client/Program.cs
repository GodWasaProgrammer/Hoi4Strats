using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
namespace Hoi4Strats.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddRadzenComponents();

            // Registrera DBService med anslutningssträngen
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddRadzenCookieThemeService(options =>
            {
                options.Name = "MyApplicationTheme"; // The name of the cookie
                options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
            });
            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            await builder.Build().RunAsync();
        }
    }
}
