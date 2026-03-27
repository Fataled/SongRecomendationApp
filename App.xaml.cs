
using AuthClient;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class App : Application
{
    private AuthClient.AuthClient _authClient; 
    private CurrentUser _currentUser;
    
    public App(AuthClient.AuthClient authClient, CurrentUser currentUser)
    {
        InitializeComponent();
        _authClient = authClient;
        _currentUser = currentUser;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    protected override async void OnStart()
    {
        base.OnStart();
        
        var refreshToken = await SecureStorage.GetAsync("refreshToken") ?? "";
        LoginResult data = await _authClient.LoginRefreshTokenAsync(refreshToken);
        if (data.IsSuccess)
        {
            _currentUser.Jwt = data.AccessToken;
            var userAsync = await _authClient.GetUserAsync(_currentUser.Jwt);
            _currentUser.Id = userAsync.GetProperty("id").GetString()!;
            await Shell.Current.GoToAsync(nameof(SongSearchPage));
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }

}