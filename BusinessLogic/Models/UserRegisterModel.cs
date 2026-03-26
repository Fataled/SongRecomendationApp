using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectHellsParadise.BusinessLogic.Models;

public class UserRegisterModel : ObservableObject
{
    private string _email;
    private string _password;
    private string _name;
    private string _totp;
    public UserRegisterModel(string name, string password, string email,string totp)
    {
        _name = name;
        _password = password;
        _email = email;
        _totp = totp ;
    }

    public UserRegisterModel()
    {
        _name = "";
        _email = "";
        _password = "";
        _totp = "";

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

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public string Totp
    {
        get => _totp;
        set => SetProperty(ref _totp, value);
    }
}