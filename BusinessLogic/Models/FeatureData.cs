namespace ProjectHellsParadise.BusinessLogic.Models;

public class FeatureData
{
    public FeatureData(string songName, string artist, float[] floats)
    {
        SongName = songName;
        Artist = artist;
        FeatureArray = floats;
    }

    public FeatureData()
    {
        SongName = "";
        Artist = "";
        FeatureArray = [];
    }

    public string SongName { get; }

    public string Artist { get; }

    public float[] FeatureArray { get; }
}