namespace ProjectHellsParadise.BusinessLogic.Models;

/// <summary>
/// A class designed to keep track of the current user
/// </summary>
/// <author>Brume Ako</author>
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