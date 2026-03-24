using System.Text.Json;
using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;
using AuthClient;

public partial class RegisterPageViewModel : ObservableObject
{
    private readonly AuthClient _authClient;
    private readonly AnalyticsClient _analyticsClient;

    public RegisterPageViewModel(AuthClient authClient, AnalyticsClient analyticsClient)
    {
        _authClient = authClient;
        _analyticsClient = analyticsClient;
    }

    public UserRegisterModel RegisterModel { get; set; } = new UserRegisterModel();

    [RelayCommand]
    private async Task RegisterViaEmailPassword(UserRegisterModel registerModel)
    {
        
        try
        {
            JsonElement response =
                await _authClient.RegisterAsync(registerModel.Email, registerModel.Password, registerModel.Name);

            RegisterModel = new UserRegisterModel();
            OnPropertyChanged(nameof(RegisterModel));
            
            await _analyticsClient.IngestEvent("User Registration", "user token goes here", properties: new Dictionary<string, object>
            {
                { "User Email", registerModel.Email },
                {"User Name",  registerModel.Name },
            }); // TODO ADD USER TOKEN
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private async Task LoginViaEmailPassword(UserLoginModel loginModel)
    {
        try
        {
            JsonElement response =
                await _authClient.LoginAsync(loginModel.Email, loginModel.Password, loginModel.TotpCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}