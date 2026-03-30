using System.Text.Json;
using ProjectHellsParadise.BusinessLogic.Exceptions;

namespace ProjectHellsParadise.BusinessLogic.APIs;
/// <summary>
/// A class that all other api based drive from to give structure and allow for less duplicated code
/// </summary>
/// <author>Brume Ako</author>
public abstract class ApiClientBase
{
    /// <summary>
    /// The http object that gets used 
    /// </summary>
    protected readonly HttpClient HttpClient;
    
    /// <summary>
    /// The base url for each api get pass through to this var
    /// </summary>
    private readonly string _baseUrl;
    
    /// <summary>
    /// Constructor for this base class
    /// </summary>
    /// <param name="baseUrl">The url address for each api without any endpoints</param>
    /// <param name="handler">This is exclusively for deezer's api and allows for specific settings for the httpclient to be set</param>
    /// 
    protected ApiClientBase(string baseUrl, HttpMessageHandler? handler = null)
    {
        HttpClient = new HttpClient();
        _baseUrl = baseUrl;
        HttpClient.BaseAddress = new Uri(baseUrl);
        
    }
        
    /// <summary>
    /// Send a GET request over http to the server set in base url
    /// </summary>
    /// <param name="endpoint">the endpoint to attach to base url</param>
    /// <param name="query">What being asked for i.e. genres or song title </param>
    /// <param name="ending">if a url has an ending i.e. playlists?limit=100</param>
    /// <typeparam name="T">The type we parse the JSON to</typeparam>
    /// <returns>A new object in the type of T</returns>
    /// <exception cref="DTOException">If a failure happens when parsing to a DTO</exception>
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
    
    /// <summary>
    /// Send a POST request to the http endpoint
    /// </summary>
    /// <param name="endpoint">the endpoint to attach to base url</param>
    /// <param name="body">The data being sent</param>
    /// <typeparam name="T">The type we parse the return JSON to</typeparam>
    /// <returns>A new object in T's type</returns>
    /// <exception cref="DTOException">For any errors that occur when trying to parse the JSON</exception>
    protected async Task<T> SendAsync<T>(string endpoint, object body)
    {
        HttpRequestMessage request = new(HttpMethod.Post, $"{_baseUrl}/{endpoint}");
        await AddContent(request, body);
        await AddAuthHeader(request);
        await AddRequestHeader(request);
        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode(); 
        string jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString)
            ?? throw new DTOException("Failed to deserialize response to " + typeof(T));
        
    }
    
    /// <summary>
    /// Genreates the GET request
    /// </summary>
    /// <param name="endpoint">the endpoint to attach to base url</param>
    /// <param name="query">What we are asking for</param>
    /// <param name="ending">more details i.e. what type of query and how many</param>
    /// <returns></returns>
    private HttpRequestMessage GenerateRequest(string endpoint, string query, string? ending = null)
    {
        string url = $"{_baseUrl}/{endpoint}/{query}{(ending != null ? $"/{ending}" : "")}";
        if (endpoint.Equals("search?q=")){
            url = $"{_baseUrl}/{endpoint}{Uri.EscapeDataString(query)}{(ending != null ? $"/{ending}" : "")}";
        }
        return new HttpRequestMessage(HttpMethod.Get, url);
    }
    
    /// <summary>
    /// Add an auth header if required (usually bearer)
    /// </summary>
    /// <param name="request">The HttpRequestMessage that we need to modify</param>
    /// <returns>The same HttpRequestMessage but with the header attached</returns>
    protected abstract Task AddAuthHeader(HttpRequestMessage request);
    
    /// <summary>
    /// Adds a request header in case the api requires one
    /// </summary>
    /// <param name="request">The HttpRequestMessage that we need to modify</param>
    /// <returns>The same HttpRequestMessage but with the header attached</returns>
    protected abstract Task AddRequestHeader(HttpRequestMessage request);
    
    /// <summary>
    /// Add the content to a POST request
    /// </summary>
    /// <param name="requestMessage">The HttpRequestMessage that we need to modify</param>
    /// <param name="body">The data we want to add to the request</param>
    /// <returns>The same HttpRequestMessage but with the body attached</returns>
    protected abstract Task AddContent(HttpRequestMessage requestMessage ,object body);
}