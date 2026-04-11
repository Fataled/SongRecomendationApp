using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.APIs;

/// <summary>
/// Author: Rithvik Ganesh Konapala
/// Handles Apple Music catalog lookup and opening songs externally.
/// </summary>
public class AppleMusic
{
    private const string DeveloperToken = "PASTE_YOUR_APPLE_MUSIC_DEVELOPER_TOKEN_HERE";
    private const string Storefront = "ca";

    private readonly HttpClient _httpClient;

    public AppleMusic()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.music.apple.com/")
        };
    }

    /// <summary>
    /// Searches Apple Music for the best match for a song query.
    /// </summary>
    public async Task<AppleMusicSong?> SearchSongAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;

        if (DeveloperToken.Contains("PASTE_YOUR_APPLE_MUSIC_DEVELOPER_TOKEN_HERE"))
            throw new InvalidOperationException("Add your Apple Music developer token in AppleMusic.cs before testing this feature.");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", DeveloperToken);

        string route =
            $"v1/catalog/{Storefront}/search?term={Uri.EscapeDataString(query)}&types=songs&limit=1";

        using HttpResponseMessage response = await _httpClient.GetAsync(route);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Apple Music request failed: {response.StatusCode} - {error}");
        }

        await using Stream responseStream = await response.Content.ReadAsStreamAsync();

        AppleMusicSearchResponse? searchResponse =
            await JsonSerializer.DeserializeAsync<AppleMusicSearchResponse>(
                responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        AppleMusicSongDto? firstSong = searchResponse?.Results?.Songs?.Data?.FirstOrDefault();

        if (firstSong?.Attributes == null)
            return null;

        return new AppleMusicSong
        {
            Id = firstSong.Id ?? string.Empty,
            Title = firstSong.Attributes.Name ?? string.Empty,
            ArtistName = firstSong.Attributes.ArtistName ?? string.Empty,
            Url = firstSong.Attributes.Url ?? string.Empty
        };
    }

    /// <summary>
    /// Finds the song in Apple Music and opens it externally.
    /// </summary>
    public async Task OpenSongAsync(string query)
    {
        AppleMusicSong? song = await SearchSongAsync(query);

        if (song == null || string.IsNullOrWhiteSpace(song.Url))
            throw new Exception("No Apple Music match was found for this recommendation.");

        await Launcher.Default.OpenAsync(song.Url);
    }
}

public class AppleMusicSearchResponse
{
    [JsonPropertyName("results")]
    public AppleMusicResults? Results { get; set; }
}

public class AppleMusicResults
{
    [JsonPropertyName("songs")]
    public AppleMusicSongsContainer? Songs { get; set; }
}

public class AppleMusicSongsContainer
{
    [JsonPropertyName("data")]
    public List<AppleMusicSongDto>? Data { get; set; }
}

public class AppleMusicSongDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("attributes")]
    public AppleMusicSongAttributes? Attributes { get; set; }
}

public class AppleMusicSongAttributes
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("artistName")]
    public string? ArtistName { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}