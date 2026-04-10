using System.Text.Json.Serialization;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class AppleMusicSongAttributesDTO
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("artistName")]
    public string? ArtistName { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}