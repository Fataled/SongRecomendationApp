using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

namespace ProjectHellsParadise.BusinessLogic.Models;

public class FeatureData
{
    private string _songName;
    private string _artist;
    private FeatureExtractionDTO _features;
    private GenrePredictionDTO[] _genre;
    
    public FeatureData(string songName, string artist, FeatureExtractionDTO DTO)
    {
        _songName = songName;
        _artist = artist;
        _features = DTO;
        _genre = [];
    }

    public FeatureData()
    {
        _songName = "";
        _artist = "";
        _features = new FeatureExtractionDTO();
        _genre = [];
    }
    
    public string SongName => _songName;

    public string Artist => _artist;

    public FeatureExtractionDTO Features => _features;
    
    public GenrePredictionDTO[] Genre => _genre;


    public void AddGenreData(GenrePredictionDTO[] DTO)
    {
        _genre = DTO;
    }

    public override string ToString()
    {
        return $"{_songName} - {_artist}: {_features} {string.Join(", ", _genre.Select(p => $"{p.label} ({p.score:P2})"))}";
    }
    
}