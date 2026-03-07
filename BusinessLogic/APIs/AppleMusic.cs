namespace ProjectHellsParadise.BusinessLogic.APIs;

public class AppleMusic : ApiClientBase
{
    public AppleMusic() : base("")
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