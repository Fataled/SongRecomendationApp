using System.Text.Json;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public abstract class APIClientBase
{
    protected readonly HttpClient _httpClient;
    protected readonly string _baseUrl;

    protected APIClientBase(string baseURL)
    {
        _httpClient = new  HttpClient();
        _baseUrl = baseURL;
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
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