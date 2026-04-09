using System.Text.Json.Serialization;

namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class AppleMusicSearchResponseDTO
{
    [JsonPropertyName("results")]
    public AppleMusicResultsDTO? Results { get; set; }
}