using System.Net.Http.Headers;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public class FeatureExtractionApi : ApiClientBase
{
    private bool _connected;
    
    public FeatureExtractionApi() : base("http://159.203.18.252:4000")
    {
        _connected = false; //TODO make it so on shutdown this becomes false or on disconnect So far we have smth but not sure if its the best idea
    }

    private async Task WaitForServer()
    {
        int maxAttempts = 10;
        int delayMs = 1000;

        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                HttpResponseMessage responseMessage = await HttpClient.GetAsync("/");
                if (responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Connection established successfully.");
                    _connected = true;
                    return;
                }
            }
            catch
            {
                
            }
            await Task.Delay(delayMs);
        }
    }
    

    public async Task<T> PostAsync<T>(string endpoint, object body)
    {
        await ReInitializeConnection();
        return await SendAsync<T>(endpoint, body);
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    protected override Task AddRequestHeader(HttpRequestMessage request)
    {
        request.Content?.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        return Task.CompletedTask;
    }

    protected override Task AddContent(HttpRequestMessage request, object body)
    {
        request.Content = new ByteArrayContent((byte[])body);
        return Task.CompletedTask;
    }

    private async Task ReInitializeConnection()
    {
        if (_connected) return;
        await WaitForServer();
    }
}