using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectHellsParadise.BusinessLogic.Models;

public class UserRegisterModel : ObservableObject
{
    private string _email;
    private string _password;
    private string _name;
    public UserRegisterModel(string name, string password, string email)
    {
        _name = name;
        _password = password;
        _email = email;
    }

    public UserRegisterModel()
    {
        _name = "";
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

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}