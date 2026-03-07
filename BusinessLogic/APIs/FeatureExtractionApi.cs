using System.Net.Http.Headers;
using System.Text.Json;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public class FeatureExtractionApi : ApiClientBase
{
    private FeatureExtractionApi() : base("http://159.203.18.252:4000")
    {
        
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
                    return;
                }
            }
            catch
            {
                
            }
            await Task.Delay(delayMs);
        }
    }

    protected override async Task<T> PostAsync<T>(string endpoint, object body)
    {
        HttpRequestMessage request = new(HttpMethod.Post, $"{BaseUrl}/{endpoint}");
        request.Content = new ByteArrayContent((byte[]) body);
        await AddAuthHeader(request);
        await AddRequestHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
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
}