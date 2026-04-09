using System.Text.Json.Serialization;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class AppleMusicSongsContainerDTO
{
    [JsonPropertyName("data")]
    public List<AppleMusicSongDTO>? Data { get; set; }
}