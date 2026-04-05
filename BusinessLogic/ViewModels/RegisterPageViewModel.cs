using System.Text.Json;
using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.ExtraStuff;
using ProjectHellsParadise.BusinessLogic.Models;
using ProjectHellsParadise.BusinessLogic.Services;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;
using AuthClient;

// TODO PUT THEIR IMAGE ABOVE PASSWORD ON LOGIN PAGE
public partial class RegisterPageViewModel : ObservableObject
{
    private readonly AuthClient _authClient;
    private readonly AnalyticsClient _analyticsClient;
    private CurrentUser _currentUser;
    private IDialogService _dialogService;
    [ObservableProperty]
    private bool _rememberMe;

    public RegisterPageViewModel(AuthClient authClient, AnalyticsClient analyticsClient, CurrentUser currentUser, IDialogService dialogService)
    {
        _authClient = authClient;
        _analyticsClient = analyticsClient;
        _currentUser = currentUser;
        _dialogService = dialogService;
        _rememberMe = false;  
    }

    public UserRegisterService RegisterService { get; set; } = new UserRegisterService();

    [RelayCommand]
    private async Task RegisterViaEmailPassword(UserRegisterService registerService)
    {
        try
        {
            JsonElement response =
                await _authClient.RegisterAsync(registerService.Email, registerService.Password, registerService.Name);

            _currentUser.Jwt = response.GetProperty("access_token").GetString()!;
            
            RegisterService = new UserRegisterService();
            OnPropertyChanged(nameof(RegisterService));
            
            JsonElement userData = await _authClient.GetUserAsync(_currentUser.Jwt);

            _currentUser.Id = userData.GetProperty("id").GetString()!;
            
            await _analyticsClient.IngestEvent("User Registration", _currentUser.Id, properties: new Dictionary<string, object>
            {
                { "User Email", registerService.Email },
                {"User Name",  registerService.Name },
            });
            
            if (RememberMe)
            {
                RefreshTokenResponse refreshTokenResponse = await _authClient.GetRefreshTokenAsync(_currentUser.Jwt);
                if (refreshTokenResponse.RefreshToken != null) await SecureStorage.SetAsync("refreshToken", refreshTokenResponse.RefreshToken);
                else { await _dialogService.ShowAlertAsync("Token error", "A token error occured you can just click ok", "ok"); }
            }

            RegisterService = new UserRegisterService();

            await Shell.Current.GoToAsync(nameof(SongSearchPage));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }   
    }

    [RelayCommand]
    private async Task RegisterViaGoogle(UserRegisterService registerService)
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
            
            await Shell.Current.GoToAsync(nameof(SongSearchPage));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private async Task CheckEmail()
    {
        bool? emailExists = await _authClient.CheckEmail(RegisterService.Email);

        switch (emailExists)
        {
            case true:
                await _dialogService.ShowAlertAsync("Email exists", "This email is connected to an account", "ok");
                break;
            case false:
                await Shell.Current.GoToAsync(nameof(AccountCreationPage));
                break;
            default:
                await _dialogService.ShowAlertAsync("Issue checking email", "Try again with a different email", "ok");
                break;
        }
    } 
    
    [RelayCommand]
    private async Task LoginViaEmailPassword(UserRegisterService loginService)
    {
        try
        {
            LoginResult response =
                await _authClient.LoginAsync(loginService.Email, loginService.Password);

            if (response.IsSuccess)
            {
                _currentUser.Jwt = response.AccessToken!;

                JsonElement userData = await _authClient.GetUserAsync(_currentUser.Jwt);

                _currentUser.Id = userData.GetProperty("id").GetString()!;

                await _analyticsClient.IngestEvent("User Logged In", _currentUser.Id,
                    properties: new Dictionary<string, object>
                    {
                        { "User Email", loginService.Email },
                    });
                
                if (RememberMe)
                {
                    RefreshTokenResponse refreshTokenResponse = await _authClient.GetRefreshTokenAsync(_currentUser.Jwt);
                    if (refreshTokenResponse.RefreshToken != null) await SecureStorage.SetAsync("refreshToken", refreshTokenResponse.RefreshToken);
                    else { await _dialogService.ShowAlertAsync("Token error", "A token error occured you can just click ok", "ok"); }
                }
                
                RegisterService = new UserRegisterService();
                
                await Shell.Current.GoToAsync(nameof(SongSearchPage));
            }
            else if (response.MfaRequired)
            {
                await Shell.Current.GoToAsync(nameof(SongSearchPage));
            }
            else
            {
                await _dialogService.ShowAlertAsync("Login Error", "There was an error logging you in", "ok");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private async Task GoRegisterPage()
    {
        RegisterService = new UserRegisterService();
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    [RelayCommand]
    private async Task GoLoginPage()
    {
        RegisterService = new UserRegisterService();
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    [RelayCommand]
    private async Task StartAccRecovery()
    {
        RegisterService = new UserRegisterService();
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }
    
}