using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.MyMath;

public class Vector
{
    private List<FeatureData> _featureDataSet;

    public Vector(List<FeatureData> featureDataSet)
    {
        _featureDataSet = featureDataSet;
    }

    private float ScaleToFloat(string scale) => scale == "major" ? 1f : 0f;

    private float KeyToFloat(string key)
    {
        var map = new Dictionary<string, float>
        {
            {"C", 0}, {"C#", 1}, {"Db", 1}, {"D", 2}, {"D#", 3}, {"Eb", 3},
            {"E", 4}, {"F", 5}, {"F#", 6}, {"Gb", 6}, {"G", 7}, {"G#", 8},
            {"Ab", 8}, {"A", 9}, {"A#", 10}, {"Bb", 10}, {"B", 11}
        };
        return map[key] / 11f; // normalize to 0-1
    }

    private float Normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    public float[][] MakeVectors()
    {
        float beatsConfidenceMin = _featureDataSet.Min(s => s.Features.beats_confidence);
        float beatsConfidenceMax = _featureDataSet.Max(s => s.Features.beats_confidence);
        float bpmMin = _featureDataSet.Min(s => s.Features.bpm);
        float bpmMax = _featureDataSet.Max(s => s.Features.bpm);
        float keyStrengthMin = _featureDataSet.Min(s => s.Features.key_strength);
        float keyStrengthMax = _featureDataSet.Max(s => s.Features.key_strength);
        float loudnessMin = _featureDataSet.Min(s => s.Features.loudness);
        float loudnessMax = _featureDataSet.Max(s => s.Features.loudness);
        float spectralCentroidMin = _featureDataSet.Min(s => s.Features.spectral_centroid);
        float spectralCentroidMax = _featureDataSet.Max(s => s.Features.spectral_centroid);


        float[][] vectorArray = _featureDataSet.Select(s => new float[]
        {
            Normalize(s.Features.beats_confidence, beatsConfidenceMin, beatsConfidenceMax),
            Normalize(s.Features.bpm,              bpmMin,             bpmMax),
            KeyToFloat(s.Features.key),
            Normalize(s.Features.key_strength,     keyStrengthMin,     keyStrengthMax),
            Normalize(s.Features.loudness,         loudnessMin,        loudnessMax),
            Normalize(s.Features.spectral_centroid,spectralCentroidMin,spectralCentroidMax),
            ScaleToFloat(s.Features.scale),
        }).ToArray();

        return vectorArray;
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0;
        float magA = 0;
        float magB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }

        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }
    
    public static List<SongSimilarity> Rank(List<FeatureData>  featureDataList)
    {
        
        
        return featureDataList.Skip(1)
            .Select((songVec, index) =>
            {
                var score = CosineSimilarity(songVec.Vector, featureDataList[0].Vector);
                return new SongSimilarity()
                {
                    Index = featureDataList[index + 1].SongName + ", " + featureDataList[index + 1].Artist,
                    Score = score,
                    AngleBetween = Math.Acos(score) * 180 / Math.PI,
                    Explanation = InterpretSimilarity(score)
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();
    }

    private static string InterpretSimilarity(double score)
    {
        return score switch
        {
            > 0.6 => "Very strong similarity",
            > 0.4 => "Noticeable similarity",
            > 0.2 => "Light similarity",
            > -0.2 => "Neutral",
            _ => "Clearly different"
        };
    }

}