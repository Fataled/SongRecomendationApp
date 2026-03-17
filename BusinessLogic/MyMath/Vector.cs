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

    private static void Normalize(List<float[]> featureVectors)
    {
        int featureCount = featureVectors[0].Length;

        float[] mins = new float[featureCount];
        float[] maxs = new float[featureCount];

        // Initialize
        for (int i = 0; i < featureCount; i++)
        {
            mins[i] = float.MaxValue;
            maxs[i] = float.MinValue;
        }

        // Find min/max
        foreach (var vec in featureVectors)
        {
            for (int i = 0; i < featureCount; i++)
            {
                if (vec[i] < mins[i]) mins[i] = vec[i];
                if (vec[i] > maxs[i]) maxs[i] = vec[i];
            }
        }
        
        foreach (var vec in featureVectors)
        {
            for (int i = 0; i < featureCount; i++)
            {
                if (maxs[i] - mins[i] == 0)
                    vec[i] = 0; // avoid divide by zero
                else
                    vec[i] = (vec[i] - mins[i]) / (maxs[i] - mins[i]);
            }
        }
    }

    private static void L2Normalize(float[] vec)
    {
        double sum = 0;
        for (int i = 0; i < vec.Length; i++)
            sum += vec[i] * vec[i];

        double mag = Math.Sqrt(sum);

        if (mag == 0) return;

        for (int i = 0; i < vec.Length; i++)
            vec[i] = (float)(vec[i] / mag);
    }


    public float[][] MakeVectors()
    {
        float[] weights = [
            0.5f,  // beats_confidence
            2.0f,  // bpm - rhythm is important
            1.0f,  // key
            1.0f,  // key_strength
            1.0f,  // loudness
            1.5f,  // spectral_centroid
            1.0f,  // scale
            2.0f,  // danceability - important
            1.0f,  // dynamic_complexity
            // mfcc - reduce these
            0.8f, 0.8f, 0.8f, 0.8f,
            0.8f, 0.8f, 0.8f, 0.8f,
            0.8f, 0.8f, 0.8f, 0.8f
        ];
        
        float[][] vectorArray = _featureDataSet.Select(s =>
        {
            float[] baseFeatures = [
                s.Features.beats_confidence,
                s.Features.bpm,
                KeyToFloat(s.Features.key),
                s.Features.key_strength,
                s.Features.loudness,
                s.Features.spectral_centroid,
                ScaleToFloat(s.Features.scale),
                s.Features.danceability,
                s.Features.dynamic_complexity
            ];

            float[] mfccFeatures = Enumerable.Range(1, 12)
                .Select(i => s.Features.mfcc[i])
                .ToArray();

            return baseFeatures.Concat(mfccFeatures).ToArray();
        }).ToArray();
        
        Normalize(vectorArray.ToList());

        try
        {
            foreach (float[] vector in vectorArray)
            {
                for (int j = 0; j < vector.Length; j++)
                {
                    vector[j] *= weights[j];
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + "AT MAKE VECTORS");
        }
        
        vectorArray.ToList().ForEach(L2Normalize);

        // normalizes everything in one pass
        return vectorArray;
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        return a.Select((t, i) => t * b[i]).Sum();
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
                    Explanation = InterpretSimilarity(score),
                    Mp3Bytes = featureDataList[index + 1].MP3SongBytes
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();
    }
    
    private static string InterpretSimilarity(double score)
    {
        return score switch
        {
            > 0.97 => "Nearly identical",
            > 0.92 => "Very strong similarity",
            > 0.85 => "Strong similarity",
            > 0.75 => "Moderate similarity",
            > 0.60 => "Weak similarity",
            _      => "Clearly different"
        };
    }
}