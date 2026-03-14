using System.Net.Http.Headers;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public class FeatureExtractionApi : ApiClientBase
{
    private bool _connected;
    
    public FeatureExtractionApi() : base("http://127.0.0.1:4000") //  SERVER: http://159.203.18.252:4000  HOME: http://127.0.0.1:4000
    {
        _connected = false; //TODO make it so on shutdown this becomes false or on disconnect So far we have smth but not sure if its the best idea
        HttpClient.Timeout = TimeSpan.FromSeconds(600);
    }

    private async Task WaitForServer()
    {
        if (_connected) return;
        
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
    

    public async Task<FeatureData> GetFeaturesAsync(string endpoint, object body)
    {
        await ReInitializeConnection();
        ByteRecord byteRecord = (ByteRecord)body;
        FeatureExtractionDTO dto = await SendAsync<FeatureExtractionDTO>(endpoint, byteRecord.PreviewBytes);
        return new FeatureData(byteRecord.Title, byteRecord.Title, dto);
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
        if (request.Content is not MultipartFormDataContent)
        {
            request.Content?.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        }
        return Task.CompletedTask;
    }

    protected override Task AddContent(HttpRequestMessage request, object body)
    {
        switch (body)
        {
            case byte[][] wavFiles:
                MultipartFormDataContent form = new MultipartFormDataContent();
                foreach (byte[] wavByte in wavFiles)
                {
                    ByteArrayContent fileContent = new ByteArrayContent(wavByte);
                    form.Add(fileContent, "files", "audio.wav");
                }
                request.Content = form;
                break;
            default:
                request.Content = new ByteArrayContent((byte[])body);
                break;
        }
        return Task.CompletedTask;
    }

    private async Task ReInitializeConnection()
    {
        if (_connected) return;
        await WaitForServer();
    }
}