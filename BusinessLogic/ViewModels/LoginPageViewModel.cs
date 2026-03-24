using System.Text.Json;
using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly AuthClient.AuthClient _authClient;
    private readonly AnalyticsClient _analyticsClient;
    private CurrentUser _currentUser;

    public LoginPageViewModel(AuthClient.AuthClient authClient, AnalyticsClient analyticsClient, CurrentUser currentUser, IConfiguration config)
    {
        _authClient = authClient;
        _analyticsClient = analyticsClient;
        _currentUser = currentUser;
    }

    public UserLoginModel LoginModel { get; set; } = new UserLoginModel();
    
    [RelayCommand]
    private async Task LoginViaEmailPassword(UserLoginModel loginModel)
    {
        try
        {
            JsonElement response =
                await _authClient.LoginAsync(loginModel.Email, loginModel.Password, loginModel.TotpCode);
            
            _currentUser.Jwt = response.GetProperty("access_token").GetString()!;

            JsonElement userData = await _authClient.GetUserAsync(_currentUser.Jwt);

            _currentUser.Id = userData.GetProperty("id").GetString()!;
            
            await _analyticsClient.IngestEvent("User Logged In", _currentUser.Id, properties: new Dictionary<string, object>
            {
                { "User Email", loginModel.Email },
            });
            
            await Shell.Current.GoToAsync(nameof(SongSearchPage));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}