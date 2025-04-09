using Blazored.LocalStorage;
using Hoi4Strats.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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


        //builder.Services.AddTransient<AuthMsgHandler>();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddApiAuthorization();
        //builder.Services.AddScoped(sp =>
        //{
        //    // H�mta beroenden fr�n DI
        //    var localStorage = sp.GetRequiredService<ILocalStorageService>();
        //    var logger = sp.GetRequiredService<ILogger<AuthMsgHandler>>();

        //    // Skapa din anpassade handler med beroenden
        //    var handler = new AuthMsgHandler(localStorage, logger)
        //    {
        //        InnerHandler = new HttpClientHandler() // Standard-handlern f�r Blazor WASM
        //    };

        //    // Skapa HttpClient med din handler och basadress
        //    var httpClient = new HttpClient(handler)
        //    {
        //        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        //    };

        //    return httpClient;
        //});
        //builder.Services.AddScoped(sp =>
        //{
        //    var handler = sp.GetRequiredService<AuthMsgHandler>();
        //    handler.InnerHandler = new HttpClientHandler();
        //    return new HttpClient(handler) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        //});
        //builder.Services.AddScoped(sp =>
        //{
        //    var handler = sp.GetRequiredService<AuthMsgHandler>();
        //    handler.InnerHandler = new HttpClientHandler(); // Standard WASM-handler
        //    return new HttpClient(handler)
        //    {
        //        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        //    };
        //});
        builder.Services.AddScoped<AuthMsgHandler>();
        builder.Services.AddScoped<AuthenticatedHttpClient>(sp =>
        {
            var handler = sp.GetRequiredService<AuthMsgHandler>();
            handler.InnerHandler = new HttpClientHandler();

            return new AuthenticatedHttpClient(new HttpClient(handler)
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });
        });
        builder.Services.AddScoped<UserService>();

        await builder.Build().RunAsync();
    }
}