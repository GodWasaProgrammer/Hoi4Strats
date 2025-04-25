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
        try
        {
            // Gör GET-förfrågan till Steam API
            var response = await client.GetAsync(requestUrl);

            // Kontrollera om förfrågan lyckades
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            // Hantera fel, returnera felmeddelande
            return null;
        }

        return null;
    }
}