using System.Text.Json;
using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.ExtraStuff;
using ProjectHellsParadise.BusinessLogic.Models;

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
            if (RememberMe)
            {
                RefreshTokenResponse refreshTokenResponse = await _authClient.GetRefreshTokenAsync(_currentUser.Jwt);
                if (refreshTokenResponse.RefreshToken != null) await SecureStorage.SetAsync("refreshToken", refreshTokenResponse.RefreshToken);
                else { await _dialogService.ShowAlertAsync("Token error", "A token error occured you can just click ok", "ok"); }
            }

            RegisterModel = new UserRegisterModel();

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
        bool? emailExists = await _authClient.CheckEmail(RegisterModel.Email);

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
    private async Task LoginViaEmailPassword(UserRegisterModel loginModel)
    {
        try
        {
            LoginResult response =
                await _authClient.LoginAsync(loginModel.Email, loginModel.Password);

            if (response.IsSuccess)
            {
                _currentUser.Jwt = response.AccessToken!;

                JsonElement userData = await _authClient.GetUserAsync(_currentUser.Jwt);

                _currentUser.Id = userData.GetProperty("id").GetString()!;

                await _analyticsClient.IngestEvent("User Logged In", _currentUser.Id,
                    properties: new Dictionary<string, object>
                    {
                        { "User Email", loginModel.Email },
                    });
                
                if (RememberMe)
                {
                    RefreshTokenResponse refreshTokenResponse = await _authClient.GetRefreshTokenAsync(_currentUser.Jwt);
                    if (refreshTokenResponse.RefreshToken != null) await SecureStorage.SetAsync("refreshToken", refreshTokenResponse.RefreshToken);
                    else { await _dialogService.ShowAlertAsync("Token error", "A token error occured you can just click ok", "ok"); }
                }
                
                RegisterModel = new UserRegisterModel();
                
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
        RegisterModel = new UserRegisterModel();
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    [RelayCommand]
    private async Task GoLoginPage()
    {
        RegisterModel = new UserRegisterModel();
        await Shell.Current.GoToAsync("///MainPage");
    }

    [RelayCommand]
    private async Task StartAccRecovery()
    {
        RegisterModel = new UserRegisterModel();
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }

    [RelayCommand]
    private async Task ForgotPassword(string email)
    {
        JsonElement resetRequest = await _authClient.RequestPasswordReset(email);
        
    } 
    
}