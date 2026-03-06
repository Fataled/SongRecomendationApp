namespace ProjectHellsParadise.BusinessLogic.APIs;

public class FeatureExtractionApi : APIClientBase
{
    public FeatureExtractionApi(string baseURL) : base(baseURL)
    {
        
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }
}