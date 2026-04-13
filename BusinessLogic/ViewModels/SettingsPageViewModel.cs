using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly AuthClient.AuthClient _authClient;
    private readonly CurrentUser _currentUser;

    [ObservableProperty]
    private bool isDarkMode;

    [ObservableProperty]
    private bool rememberMe;

    public SettingsPageViewModel(AuthClient.AuthClient authClient, CurrentUser currentUser)
    {
        _authClient = authClient;
        _currentUser = currentUser;
    }
    [RelayCommand]
    private async Task GoFavoritesPage()
    {
        await Shell.Current.GoToAsync(nameof(FavoritesPlaylistPage));
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authClient.LogoutAsync(_currentUser.Jwt);
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}