using System.Text.Json;
using ProjectHellsParadise.BusinessLogic.Exceptions;

namespace ProjectHellsParadise.BusinessLogic.APIs;

public abstract class ApiClientBase
{
    protected readonly HttpClient HttpClient;
    protected readonly string BaseUrl;

    protected ApiClientBase(string baseUrl, HttpMessageHandler? handler = null)
    {
        HttpClient = new HttpClient();
        BaseUrl = baseUrl;
        HttpClient.BaseAddress = new Uri(baseUrl);
        
    }

    protected async Task<T> RequestAsync<T>(string endpoint, string query, string? ending = null)
    {
        HttpRequestMessage request = GenerateRequest(endpoint, query, ending);
        await AddAuthHeader(request);
        await AddRequestHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json)
               ?? throw new DTOException("Failed to deserialize response to " + typeof(T));
    }

    protected async Task<T> SendAsync<T>(string endpoint, object body)
    {
        HttpRequestMessage request = new(HttpMethod.Post, $"{BaseUrl}/{endpoint}");
        await AddContent(request, body);
        await AddAuthHeader(request);
        await AddRequestHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode(); 
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json)
            ?? throw new DTOException("Failed to deserialize response to " + typeof(T));
        
    }

    private HttpRequestMessage GenerateRequest(string endpoint, string query, string? ending = null)
    {
        string url = $"{BaseUrl}/{endpoint}/{query}{(ending != null ? $"/{ending}" : "")}";
        if (endpoint.Equals("search?q=")){
            url = $"{BaseUrl}/{endpoint}{Uri.EscapeDataString(query)}{(ending != null ? $"/{ending}" : "")}";
        }
        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    protected abstract Task AddAuthHeader(HttpRequestMessage request);
    
    protected abstract Task AddRequestHeader(HttpRequestMessage request);
    
    protected abstract Task AddContent(HttpRequestMessage requestMessage ,object body);
}