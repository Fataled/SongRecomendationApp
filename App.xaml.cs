
using AuthClient;

namespace ProjectHellsParadise;

public partial class App : Application
{
    private AuthClient.AuthClient _authClient; 
    
    public App(AuthClient.AuthClient authClient)
    {
        InitializeComponent();
        _authClient = authClient;
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
            await Shell.Current.GoToAsync(nameof(SongSearchPage));
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }

}