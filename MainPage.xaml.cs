using System.Diagnostics;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise;

public partial class MainPage : ContentPage
{
    int count = 0;
    private DeezerClient _deezerClient;
    private FeatureExtractionApi _myApi;
    
    public MainPage(DeezerClient deezerClient, FeatureExtractionApi featureExtractionApi)
    {
        InitializeComponent();
        _myApi = featureExtractionApi;
        _deezerClient = deezerClient;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        DeezerDTO trackData = await _deezerClient.GetGenreSongsAsync("rap");
        
        Stopwatch sw = Stopwatch.StartNew();
        
       try{
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        

    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        CounterBtn.Text = count == 1 ? $"Clicked {count} time" : $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
        }
}