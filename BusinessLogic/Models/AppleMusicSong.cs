namespace ProjectHellsParadise.BusinessLogic.Models;

/// <summary>
/// Author: Rithvik Ganesh Konapala
/// Represents a song found through Apple Music.
/// </summary>
public class AppleMusicSong
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string ArtistName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Title} - {ArtistName}";
    }
}