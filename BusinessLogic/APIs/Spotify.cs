using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.APIs;


/// <summary>
/// Talks to the Spotify Web API
/// Inherits from the ApiClientBase class
/// </summary>
public class SpotifyClient : ApiClientBase
{
    private const string ClientId = "df13c3929f934878af76c8286403e84d";
    private const string ClientSecret = "6c8c35b7d15244879e6274d58c039356";

    private string _bearerToken = string.Empty;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public SpotifyClient() : base("https://api.spotify.com/v1")
    { 
        
    }

    private async Task EnsureTokenAsync()
    {
        if (!string.IsNullOrEmpty(_bearerToken) && DateTime.UtcNow < _tokenExpiry)
            return;

        string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId} : {ClientSecret}"));

        HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

        tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        tokenRequest.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        HttpResponseMessage response = await HttpClient.SendAsync(tokenRequest);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new ApiException($"Spotify token fetch failed [{(int)response.StatusCode}]: {error}.");
        }

        string json = await response.Content.ReadAsStringAsync();
        SpotifyTokenResponse? tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponse>(json)
            ?? throw new DTOException("Failed to deserialize Spotify token response");

        _bearerToken = tokenResponse.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30); 
    }


    protected override async Task AddAuthHeader(HttpRequestMessage request)
    {
        await EnsureTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
    }

    protected override Task AddRequestHeader(HttpRequestMessage request) => Task.CompletedTask;

    protected override Task AddContent(HttpRequestMessage requestMessage, object body) => Task.CompletedTask;

    //Searches Spotify for tracks matching the query string.
    public async Task<List<SpotifyMusicSong>> SearchTracksAsync(string query, int limit = 20)
    {
        if (string.IsNullOrEmpty(query))
            return new List<SpotifyMusicSong>();

        await EnsureTokenAsync();

        string url = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit={limit}";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        await AddAuthHeader(request);

        HttpResponseMessage response = await HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new ApiException($"Spotify search failed [{(int)response.StatusCode}]: {error}.");
        }

        string json = await response.Content.ReadAsStringAsync();
        SpotifyDTO? dto = JsonSerializer.Deserialize<SpotifyDTO>(json)
            ?? throw new DTOException("Failed to deserialize Spotify search response");

        return dto.Tracks?.Items
            .Select(item => item.ToSpotifyMusicSong())
            .ToList() ?? new List<SpotifyMusicSong>();
    }

    public static async Task OpenInSpotifyAsync(SpotifyMusicSong song)
    {
        //Use external URL because it works on all platfroms
        //if spotify is installed it will intercept the link and open the app
        //if not, the browser opens Spotify Web Player instead.

        string url = !string.IsNullOrEmpty(song.ExternalUrl)
            ? song.ExternalUrl : $"https://open.spotify.com/track/{song.Id}";

        await Launcher.OpenAsync(new Uri(url));
    }

    //Used only for deserializing the spotify token endpoint response
    private class SpotifyTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}