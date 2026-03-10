namespace ProjectHellsParadise.BusinessLogic.Models;

public class FeatureData
{
    private string _songName;
    private string _artist;
    private float[] _featureArray;

    public FeatureData(string songName, string artist, float[] floats)
    {
        _songName = songName;
        _artist = artist;
        _featureArray = floats;
    }
    
    public string SongName { get; }
    public string Artist { get; }
    public float[] FeatureArray { get; }
}