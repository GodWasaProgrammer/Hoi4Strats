using FindRazorSourceFile.WebAssembly;
using Hoi4Strats.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

namespace Hoi4Strats.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddRadzenComponents();

        var s = builder.HostEnvironment.BaseAddress;
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddRadzenCookieThemeService(options =>
        {
            options.Name = "MyApplicationTheme"; // The name of the cookie
            options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
            options.IsSecure = true;
        });
        builder.Services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
        builder.Services.AddApiAuthorization();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddSingleton<DataService>();
        
        builder.UseFindRazorSourceFile();

        await builder.Build().RunAsync();
    }
}