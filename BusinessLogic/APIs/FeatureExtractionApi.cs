using System.Net.Http.Headers;
using System.Text.Json;
using ProjectHellsParadise.BusinessLogic.Exceptions;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public class FeatureExtractionApi : ApiClientBase
{
    private bool _connected;
    
    private FeatureExtractionApi() : base("http://159.203.18.252:4000")
    {
        _connected = false; //TODO make it so on shutdown this becomes false or on disconnect So far we have smth but not sure if its the best idea
    }

    public static async Task<FeatureExtractionApi> CreateAsync()
    {
        FeatureExtractionApi instance = new FeatureExtractionApi();
        await instance.WaitForServer();
        return instance;
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

    protected override async Task<T> RequestAsync<T>(string endpoint, object body)
    {
        ReInitializeConnection();
        HttpRequestMessage request = new(HttpMethod.Post, $"{BaseUrl}/{endpoint}");
        request.Content = new ByteArrayContent((byte[]) body);
        await AddAuthHeader(request);
        await AddRequestHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request) ?? throw new HttpRequestException("Error requesting feature extraction.");
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json)
               ?? throw new DTOException("Failed to deserialize response to " + typeof(T));
    }

    public async Task<T> PostAsync<T>(string endpoint, object body)
    {
        return await RequestAsync<T>(endpoint, body);
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    protected override Task AddRequestHeader(HttpRequestMessage request)
    {
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        return Task.CompletedTask;
    }

    private async Task ReInitializeConnection()
    {
        if (_connected) return;
        await WaitForServer();
    }
}