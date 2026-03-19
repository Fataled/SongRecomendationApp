namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public record FeatureExtractionDTO
{
    public float beats_confidence { get; set; }
    public float bpm { get; set; }
    public string key { get; set; }
    public float key_strength { get; set; }
    public float loudness { get; set; }
    public float spectral_centroid { get; set; }
    public string scale { get; set; }
    public float danceability { get; set; }
    public float dynamic_complexity { get; set; }
    public float[] mfcc { get; set; }
    public float vocal { get; set; }

    public override string ToString()
    {
        return $"beats confidence: {beats_confidence}, bpm: {bpm}, key: {key}, key strength: {key_strength}, loudness: {loudness}, spectral centroid: {spectral_centroid}, scale: {scale},  danceability: {danceability},  dynamic complexity: {dynamic_complexity}";
    }
}

