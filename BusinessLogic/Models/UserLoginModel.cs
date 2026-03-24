using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;


namespace ProjectHellsParadise.BusinessLogic.Models;
using AuthClient;
public class UserLoginModel : ObservableObject
{
    private string _email;
    private string _password;
    private string _totpCode;
    public UserLoginModel(string totpCode, string password, string email)
    {
        _totpCode = totpCode;
        _password = password;
        _email = email;
    }

    public UserLoginModel()
    {
        _totpCode = "";
        _email = "";
        _password = "";

    }
    
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string TotpCode
    {
        get => _totpCode;
        set => SetProperty(ref _totpCode, value);
    }
    
}