namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public class CurrentUser
{
    private string _jwt = "";
    private string _id = "";

    public string Jwt
    {
        get => _jwt;
        set => _jwt = value;
    }
    
    public string Id
    {
        get => _id;
        set => _id = value;
    }
}