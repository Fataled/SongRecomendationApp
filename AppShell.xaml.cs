namespace ProjectHellsParadise;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(SongSearchPage), typeof(SongSearchPage));
        Routing.RegisterRoute(nameof(Recommendation), typeof(Recommendation));
        Routing.RegisterRoute(nameof(AnalysisPage),  typeof(AnalysisPage));
    }
}