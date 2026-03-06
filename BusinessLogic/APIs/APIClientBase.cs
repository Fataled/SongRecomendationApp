using System.Text.Json;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public abstract class ApiClientBase
{
    protected readonly HttpClient HttpClient;
    protected readonly string BaseUrl;

    protected ApiClientBase(string baseUrl)
    {
        HttpClient = new  HttpClient();
        BaseUrl = baseUrl;
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/{endpoint}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
    }

    protected async Task<T> PostAsync<T>(string endpoint, object body)
    {
        return default;
    }

    protected abstract Task AddAuthHeader(HttpRequestMessage request);

}