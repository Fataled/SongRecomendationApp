using System.Text.Json.Serialization;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class DeezerDTO
{

    [JsonPropertyName("data")] 
    public DeezerTrack[] Data { get; set; } = Array.Empty<DeezerTrack>();

    public class DeezerTrack
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("artist")]
        public DeezerArtist Artist { get; set; } = new();

        [JsonPropertyName("preview")]
        public string Preview { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Title: {Title} Artist Name: {Artist.Name}";
        }
    }

    public class DeezerArtist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public override string ToString()
    {
        return Data.Length == 0
            ? "No tracks found"
            : string.Join("\n", Data.Select(t => $"Track: {t.Title}, Artist Name: {t.Artist?.Name ?? "Unknown"}"));
    }
}