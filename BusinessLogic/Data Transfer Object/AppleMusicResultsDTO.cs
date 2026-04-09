using System.Text.Json.Serialization;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class AppleMusicResultsDTO
{
    [JsonPropertyName("songs")]
    public AppleMusicSongsContainerDTO? Songs { get; set; }
}