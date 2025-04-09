using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

public class AuthMsgHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthMsgHandler> _logger;
    private readonly IJSRuntime _jsRuntime;
    private bool? _isPrerendering;

    public AuthMsgHandler(
        ILocalStorageService localStorage,
        ILogger<AuthMsgHandler> logger,
        IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (await IsPrerenderingAsync())
        {
            return await base.SendAsync(request, cancellationToken);
        }

        try
        {
            var userId = await _localStorage.GetItemAsync<string>("currentUserId");
            var token = await _localStorage.GetItemAsync<string>($"authToken_{userId}");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding authorization header: {ex.Message}");
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<bool> IsPrerenderingAsync()
    {
        if (_isPrerendering.HasValue)
            return _isPrerendering.Value;

        try
        {
            // Försök hämta ett enkelt värde från JS
            await _jsRuntime.InvokeAsync<object>("eval", "0");
            _isPrerendering = false;
        }
        catch
        {
            _isPrerendering = true;
        }

        return _isPrerendering.Value;
    }
}
