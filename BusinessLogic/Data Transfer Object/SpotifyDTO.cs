using System.Text.Json.Serialization;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

/// <summary>
/// Data transer object that mirrors the JSON structure returned by
/// Spotify GET
/// </summary>
/// <author> Obaid Waqas </author>
public class SpotifyDTO
{
    [JsonPropertyName("tracks")]
    public TracksWrapper? Tracks { get; set; }

    public class TracksWrapper
    {
        [JsonPropertyName("items")]
        public List<TrackItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Represents a single raw track item as reutned by the spotify search API.
    /// COntains all fields needed to build a SpotifyMusicSong Model.
    /// </summary>
    public class TrackItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; }

        [JsonPropertyName("explicit")]
        public bool IsExplicit {  get; set; }

        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }

        [JsonPropertyName("preview_url")]
        public string? PreviewUrl { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonPropertyName("artists")]
        public List<Artist> Artists { get; set; } = new();

        [JsonPropertyName("album")]
        public Album? Album { get; set; }

        /// <summary>
        /// Converts raw spotify json item into the SpoifyMusicSong model
        /// </summary>
        /// <returns></returns>
        public SpotifyMusicSong ToSpotifyMusicSong() => new SpotifyMusicSong
        {
            Id = Id,
            Name = Name,
            Artist = Artists.FirstOrDefault()?.Name ?? "Unknown Artist",
            Artists = string.Join(", ", Artists.Select(a => a.Name)),
            Album = Album?.Name ?? string.Empty,
            AlbumArtUrl = Album?.Images?.FirstOrDefault()?.Url ?? string.Empty,
            DurationMs = DurationMs,
            SpotifyUri = Uri,
            ExternalUrl = ExternalUrls?.Spotify ?? string.Empty,
            Popularity = Popularity,
            PreviewUrl = PreviewUrl,
            IsExplicit = IsExplicit,
        };
    }

    /// <summary>
    /// Represents a spotify artist
    /// </summary>
    public class Artist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the album a track belongs to
    /// </summary>
    public class Album
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("images")]
        public List<AlbumImage> Images { get; set; } = new();
    }

    /// <summary>
    /// Represents a single album artwork image returned by SPotify
    /// </summary>
    public class AlbumImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// contains the external web URLs for a Spotify Track
    /// </summary>
    public class ExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string Spotify { get; set; } = string.Empty;
    }
}
