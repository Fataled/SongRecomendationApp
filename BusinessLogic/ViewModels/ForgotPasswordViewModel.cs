using AnalyticsPipeline;
using AuthClient;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.ExtraStuff;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class ForgotPasswordViewModel : ObservableObject
{
    [ObservableProperty]
    private string _email;
    [ObservableProperty]
    private string _password;

    private AuthClient.AuthClient _authClient;
    private AnalyticsClient _analyticsClient;
    private IDialogService _dialogService;

    public ForgotPasswordViewModel(AuthClient.AuthClient authClient, AnalyticsClient analyticsClient, IDialogService dialogService)
    {
        _email = "";
        _password = "";
        _authClient = authClient;
        _analyticsClient = analyticsClient;
        _dialogService = dialogService;

    }


    [RelayCommand]
    private async Task SendRecoveryCode()
    {
        try
        {
           ConfirmationResponse response = await _authClient.RequestPasswordReset(_email);
           await _analyticsClient.IngestEvent("Password reset", _email);
          // await Shell.Current.GoToAsync(nameof()); TODO design the page where you actually change your password
        }
        catch (Exception e)
        {
            await _dialogService.ShowAlertAsync("Password Reset Error",
                "An error has occured when trying to send that email", "ok");
        }
    }
    
}