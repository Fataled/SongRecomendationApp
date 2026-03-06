namespace ProjectHellsParadise.BusinessLogic.APIs;

public class SpotifyClient : APIClientBase
{
    public SpotifyClient(HttpClient httpClient, string baseURL) : base("https://api.spotify.com/v1")
    {
        
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }
}