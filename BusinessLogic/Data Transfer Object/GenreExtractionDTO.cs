namespace ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

public class GenreExtractionDTO
{
    public string name { get; set; }
    public List<Predictions> preds {get; set;}

    public class Predictions
    {
        public string label { get; set; }
        public float score { get; set; }
    }

    public override string ToString()
    {
        return $"Name: {name}, Predictions: {string.Join(",", preds)}";
    }

    public string SmallString()
    {
        return $"Predictions: {string.Join(",", preds)}";
    }
}