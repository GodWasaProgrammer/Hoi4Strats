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
        uint count = 1;       // Antal nyheter att hämta
        uint maxLength = 400000;  // Max längd på innehållet

        // Skapa URL med parametrar för Hearts of Iron IV
        var requestUrl = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={appId}&maxlength={maxLength}&count={count}&format=json";
        var requestUrl2 = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={appId}&maxlength={maxLength}&count={count}&format=xml";
        var requestUrl3 = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={appId}&maxlength={maxLength}&count={count}&format=vdf";
        try
        {
            // Gör GET-förfrågan till Steam API
            var response = await client.GetAsync(requestUrl);
            var response2 = await client.GetAsync(requestUrl2);
            var response3 = await client.GetAsync(requestUrl3);
            // Kontrollera om förfrågan lyckades
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonResponse2 = await response.Content.ReadAsStringAsync();
            var jsonResponse3 = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"API Response: {jsonResponse}");
            //Console.WriteLine(jsonResponse2);
            // Returnera svar som sträng
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            // Hantera fel, returnera felmeddelande
            return null;
        }
    }
}

