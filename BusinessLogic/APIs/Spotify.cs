namespace ProjectHellsParadise.BusinessLogic.APIs;

public class SpotifyClient : ApiClientBase
{
    public SpotifyClient() : base("https://api.spotify.com/v1")
    {
        
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }

    protected override Task AddRequestHeader(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }
}