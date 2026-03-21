using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.Services;

public class SongSessionService
{
    public FeatureData BaseSong { get; set; }
    public SongSimilarity SelectedSong { get; set; }
    public List<SongSimilarity> Results { get; set; }
}