namespace ProjectHellsParadise.BusinessLogic.APIs;

/// <summary>
/// RITHVIK DO THIS AND DO THE APPLE MUSIC CONNECTION PAGE ALSO MAKE THE SETTINGS XAML HAVE TABS: ONE FOR SPOTIFY ONE FOR APPLE ONE FOR USER
/// </summary>
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

    protected override Task AddContent(HttpRequestMessage requestMessage, object body)
    {
        throw new NotImplementedException();
    }
}