using System.Text.Json.Serialization;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class SpotifyDTO
{
    [JsonPropertyName("tracks")]
    public TracksWrapper? Tracks { get; set; }

    public class TracksWrapper
    {
        [JsonProperyName("items")]
        public List<TrackItem> Items { get; set; } = new();
    }

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
        public string Uri { get; set; }

        [JsonPropertyName("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonPropertyName("artists")]
        public List<Artist> Artists { get; set; } = new();

        [JsonPropertyName("album")]
        public Album? Album { get; set; }

        //Converts raw spotify json item into the SpoifyMusicSong model
        public SpotifyMusicSong ToSpotifyMusicSong() => new SpotifyMusicSong
        {
            Id = Id,
            Name = Name,
            Artist = Artists.FirstOrDefault()?.Name ?? "Unknown Artist"
            Artists = string.Join(", ", Artists.Select(a => a.Name)),
            Album = Album?.Name ?? string.Empty,
            AlbumArtUrl = Album?.Images?.FirstOrDefault()?.Url ?? string.Empty,
            DurationMs = DurationMs,
            SpotifyUri = Uri,
            ExternalUrls = ExternalUrls?.Spotify ?? string.Empty,
            Popularity = Popularity,
            PreviewUrl = PreviewUrl,
            IsExplicit = IsExplicit,
        };
    }

    public class Artist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Album
    {
        [JsonPropertyName("album")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("images")]
        public List<AlbumImage> Images { get; set; } = new();
    }

    public class AlbumImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class ExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string Spotify { get; set; } = string.Empty;
    }
}
