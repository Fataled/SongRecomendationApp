using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

namespace ProjectHellsParadise.BusinessLogic.Models;

public class FeatureData
{
    private string _songName;
    private string _artist;
    private FeatureExtractionDTO _features;
    private GenreExtractionDTO _genre;
    
    public FeatureData(string songName, string artist, FeatureExtractionDTO DTO)
    {
        _songName = songName;
        _artist = artist;
        _features = DTO;
        _genre =  new  GenreExtractionDTO();
    }

    public FeatureData()
    {
        _songName = "";
        _artist = "";
        _features = new FeatureExtractionDTO();
        _genre = new GenreExtractionDTO();
    }
    
    public string SongName => _songName;

    public string Artist => _artist;

    public FeatureExtractionDTO Features => _features;
    
    public GenreExtractionDTO Genre => _genre;


    public void AddGenreData(GenreExtractionDTO DTO)
    {
        _genre = DTO;
    }

    public override string ToString()
    {
        return $"{_songName} - {_artist}:  {_features} {_genre.SmallString()}";
    }
    
    
    
    
}