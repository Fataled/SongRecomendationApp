using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

namespace ProjectHellsParadise.BusinessLogic.Models;

public class FeatureData
{
    private string _songName;
    private string _artist;
    private FeatureExtractionDTO _features;
    private GenrePredictionDTO[] _genre;
    private byte[] _songBytes;
    private float[] _vector;
    
    public FeatureData(string songName, string artist, FeatureExtractionDTO DTO, byte[] songBytes)
    {
        _songName = songName;
        _artist = artist;
        _features = DTO;
        _songBytes = songBytes;
        _genre = [];
    }

    public FeatureData()
    {
        _songName = "";
        _artist = "";
        _features = new FeatureExtractionDTO();
        _songBytes = [];
        _genre = [];
    }
    
    public string SongName => _songName;

    public string Artist => _artist;

    public FeatureExtractionDTO Features => _features;
    
    public GenrePredictionDTO[] Genre { get => _genre; set => _genre = value; }
    
    public byte[] SongBytes => _songBytes;
    
    public float[] Vector { get => _vector; set => _vector = value; }

    public override string ToString()
    {
        return $"{_songName} - {_artist}: {_features} {string.Join(", ", _genre.Select(p => $"{p.label} ({p.score:P2})"))}";
    }
    
}