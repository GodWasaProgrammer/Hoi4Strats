namespace Hoi4Strats;
public class SteamApiClient
{
    private static readonly HttpClientHandler handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
    private static readonly HttpClient client = new HttpClient(handler);

    public async Task<string> GetNewsForHeartsOfIronAsync()
    {
        uint appId = 394360;  // Hearts of Iron IV appid
        uint count = 5;       // Antal nyheter att hämta
        uint maxLength = 300;  // Max längd på innehållet

        // Skapa URL med parametrar för Hearts of Iron IV
        var requestUrl = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={appId}&maxlength={maxLength}&count={count}";

        try
        {
            // Gör GET-förfrågan till Steam API
            var response = await client.GetAsync(requestUrl);

            // Kontrollera om förfrågan lyckades
            response.EnsureSuccessStatusCode();

            // Returnera svar som sträng
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            // Hantera fel, returnera felmeddelande
            return $"Request error: {e.Message}";
        }
    }
}

