namespace ProjectHellsParadise.BusinessLogic.APIs;

public class FeatureExtractionApi : ApiClientBase
{
    public FeatureExtractionApi() : base("http://159.203.18.252:4000")
    {
        
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }
}