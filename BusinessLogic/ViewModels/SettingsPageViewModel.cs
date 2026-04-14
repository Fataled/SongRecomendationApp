using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

/// <summary>
/// The methods for the Settings page
/// </summary>
/// <author>Brume Ako</author>
public partial class SettingsPageViewModel : ObservableObject
{
    private readonly AuthClient.AuthClient authClient;
    private readonly CurrentUser currentUser;

    [ObservableProperty]
    private bool isAppleSignedIn;

    [ObservableProperty]
    private bool isSpotifySignedIn;

    [ObservableProperty]
    private string appleStatus = "Not connected to Apple Music";

    [ObservableProperty]
    private string spotifyStatus = "Not connected to Spotify";

    public SettingsPageViewModel(AuthClient.AuthClient authClient, CurrentUser currentUser)
    {
        this.authClient = authClient;
        this.currentUser = currentUser;
    }

    [RelayCommand]
    private async Task AppleSignIn()
    {
        try
        {
            await Browser.Default.OpenAsync(new Uri("https://music.apple.com/"), BrowserLaunchMode.SystemPreferred);
            IsAppleSignedIn = true;
            AppleStatus = "Apple Music sign-in page opened";
        }
        catch (Exception ex)
        {
            AppleStatus = $"Apple Music sign-in failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AppleSignOut()
    {
        try
        {
            await Browser.Default.OpenAsync(new Uri("https://music.apple.com/settings"), BrowserLaunchMode.SystemPreferred);
            IsAppleSignedIn = false;
            AppleStatus = "Apple Music settings opened";
        }
        catch (Exception ex)
        {
            AppleStatus = $"Apple Music sign-out failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SpotifySignIn()
    {
        try
        {
            await Browser.Default.OpenAsync(new Uri("https://accounts.spotify.com/en/login"), BrowserLaunchMode.SystemPreferred);
            IsSpotifySignedIn = true;
            SpotifyStatus = "Spotify sign-in page opened";
        }
        catch (Exception ex)
        {
            SpotifyStatus = $"Spotify sign-in failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SpotifySignOut()
    {
        try
        {
            await Browser.Default.OpenAsync(new Uri("https://www.spotify.com/account/overview/"), BrowserLaunchMode.SystemPreferred);
            IsSpotifySignedIn = false;
            SpotifyStatus = "Spotify account page opened";
        }
        catch (Exception ex)
        {
            SpotifyStatus = $"Spotify sign-out failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        await authClient.LogoutAsync(currentUser.Jwt);
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}