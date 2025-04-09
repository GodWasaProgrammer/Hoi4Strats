namespace Hoi4Strats.Client.Services
{
    public class AuthenticatedHttpClient
    {
        public HttpClient Client { get; }

        public AuthenticatedHttpClient(HttpClient httpClient)
        {
            Client = httpClient;
        }
    }
}
