using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly AuthClient.AuthClient _authClient;
    private CurrentUser _currentUser;

    public SettingsPageViewModel(AuthClient.AuthClient authClient, CurrentUser currentUser)
    {
        _authClient = authClient;
        _currentUser = currentUser;
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authClient.LogoutAsync(_currentUser.Jwt);
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}