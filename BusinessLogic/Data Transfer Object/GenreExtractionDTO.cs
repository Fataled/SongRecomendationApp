namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;


public class GenrePredictionDTO
{
    public required string label { get; set; }
    public float score { get; set; }
    
    public override string ToString()
    {
        return $"string.Join({label} ({score:P2})";
    }
}

    
