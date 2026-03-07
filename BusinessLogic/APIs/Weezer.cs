namespace ProjectHellsParadise.BusinessLogic.APIs;

public class WeezerClient : ApiClientBase
{
    public WeezerClient() : base("https://api.deezer.com")
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