using System.Text.Json;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public abstract class ApiClientBase
{
    protected readonly HttpClient HttpClient;
    protected readonly string BaseUrl;

    protected ApiClientBase(string baseUrl)
    {
        HttpClient = new HttpClient();
        BaseUrl = baseUrl;
        HttpClient.BaseAddress = new Uri(baseUrl);
    }

    protected async Task<T> GetAsync<T>(string endpoint, string query)
    {
        HttpRequestMessage request = new(HttpMethod.Get, $"{BaseUrl}/{endpoint}" + Uri.EscapeDataString(query));
        await AddAuthHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
    }

    protected virtual async Task<T> PostAsync<T>(string endpoint, object body)
    {
        HttpRequestMessage request = new(HttpMethod.Post, $"{BaseUrl}/{endpoint}");
        await AddAuthHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
        
    }

    protected abstract Task AddAuthHeader(HttpRequestMessage request);
    
    protected abstract Task AddRequestHeader(HttpRequestMessage request);
}