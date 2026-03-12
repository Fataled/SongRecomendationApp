namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public record FeatureExtractionDTO
{
    public required float[] Embedding { get; set; }
}