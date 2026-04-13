using System.Text.Json.Serialization;

namespace ProjectHellsParadise.BusinessLogic.Models;

/// <summary>
/// Represents a single spotify track shown in search results
/// </summary>
public class SpotifyMusicSong
{
    // The unique Spotify track ID
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    // Song Title
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Primary Artist
    public string Artist { get; set; } = string.Empty;

    // All artists joined by ", " for multi-artist
    public string Artists {  get; set; } = string.Empty;

    // Album Name
    public string Album {  get; set; } = string.Empty;

    // Album art image URL
    public string AlbumArtUrl {  get; set; } = string.Empty;

    //Duration in milliseconds from spotify
    [JsonPropertyName("duration_ms")]
    public int DurationMs { get; set; }

    //Human readable duration
    public string Duration => FormatDuration(DurationMs);

    //30 second preview - Null if spotify doesnt provide with one
    public string? PreviewUrl {  get; set; }

    // The URI used to open the app on this exact track
    public string SpotifyUri {  get; set; } = string.Empty;

    // The https://open.spotify.com/track/URL - browser fallback
    public string ExternalUrl {  get; set; } = string.Empty;

    // Popularity 0-100
    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    //Whether spotify marks this track as explicit
    [JsonPropertyName("explicit")]
    public bool IsExplicit { get; set; }

    //Shown in the CollectionView: "SOng Name & Artist"
    public string DisplayLabel => $"{Name} {Artist}";

    /// <summary>
    /// Converts a duration in ms into a readable "m:s" string
    /// </summary>
    /// <param name="ms"></param>
    /// <returns> 0: 00 if the value is null or negative </returns>
    private static string FormatDuration (int ms)
    {
        if (ms <= 0)
        {
            return "0:00";
        }
        int totalSeconds = ms / 1000;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes}:{seconds:D2}";
    }
}
