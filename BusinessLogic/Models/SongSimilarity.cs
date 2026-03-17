namespace ProjectHellsParadise.BusinessLogic.Models;

public class SongSimilarity
{
    public required string Index { get; init; }
    public required double Score { get; init; }
    public required double AngleBetween { get; init; }
    
    public required string Explanation { get; init; }
    
    public required byte[] Mp3Bytes { get; init; }

    public override string ToString()
    {
        return $"Index: {Index} Score: {Score} Angle Between: {AngleBetween :F2} Explanation: {Explanation}";
    }
}