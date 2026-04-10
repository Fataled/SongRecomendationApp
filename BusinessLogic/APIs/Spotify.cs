namespace ProjectHellsParadise.BusinessLogic.APIs;


/// <summary>
/// Talks to the Spotify Web API
/// Inherits from the ApiClientBase class
/// </summary>
public class SpotifyClient : ApiClientBase
{
    private string _token = "";

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

    protected override Task AddContent(HttpRequestMessage requestMessage, object body)
    {
        throw new NotImplementedException();
    }
}