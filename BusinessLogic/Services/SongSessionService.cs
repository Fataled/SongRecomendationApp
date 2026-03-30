using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.Services;
/// <summary>
/// An object to allow the transer of recommendation deatils between pages
/// </summary>
/// <author>Brume Ako</author>
public class SongSessionService
{
    public FeatureData BaseSong { get; set; }
    public SongSimilarity SelectedSong { get; set; }
    public List<SongSimilarity> Results { get; set; }
}