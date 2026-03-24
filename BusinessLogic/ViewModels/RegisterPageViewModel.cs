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
    private CurrentUser _currentUser;

    public RegisterPageViewModel(AuthClient authClient, AnalyticsClient analyticsClient, CurrentUser currentUser)
    {
        _authClient = authClient;
        _analyticsClient = analyticsClient;
        _currentUser = currentUser;
    }

    public UserRegisterModel RegisterModel { get; set; } = new UserRegisterModel();

    [RelayCommand]
    private async Task RegisterViaEmailPassword(UserRegisterModel registerModel)
    {
        try
        {
            JsonElement response =
                await _authClient.RegisterAsync(registerModel.Email, registerModel.Password, registerModel.Name);

            _currentUser.Jwt = response.GetProperty("access_token").GetString()!;
            
            RegisterModel = new UserRegisterModel();
            OnPropertyChanged(nameof(RegisterModel));
            
            JsonElement userData = await _authClient.GetUserAsync(_currentUser.Jwt);

            _currentUser.Id = userData.GetProperty("id").GetString()!;
            
            await _analyticsClient.IngestEvent("User Registration", _currentUser.Id, properties: new Dictionary<string, object>
            {
                { "User Email", registerModel.Email },
                {"User Name",  registerModel.Name },
            });

            await Shell.Current.GoToAsync(nameof(SongSearchPage));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }   
    }

    [RelayCommand]
    private async Task RegisterViaGoogle(UserRegisterModel registerModel)
    {
        try
        {
            JsonElement response = await _authClient.LoginWithOidc("google");
            
            _currentUser.Jwt = response.GetProperty("access_token").GetString()!;
            
            JsonElement userData = await _authClient.GetUserAsync(_currentUser.Jwt);
            
            _currentUser.Id = userData.GetProperty("id").GetString()!;
            
            await _analyticsClient.IngestEvent("Login Via Google", _currentUser.Id, properties: new Dictionary<string, object>
            {
                { "User Email", userData.GetProperty("email").GetString()! },
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
}