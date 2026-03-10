namespace ProjectHellsParadise;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(SongSearchPage), typeof(SongSearchPage));
    }
}