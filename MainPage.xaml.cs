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
        
        try
        {
            sw = Stopwatch.StartNew();
            ByteRecord[] wavTry2 = await _deezerClient.DownloadPreviewBytes(trackData);

            FeatureData warmUp = await _myApi.GetFeaturesAsync("features", wavTry2[0]);
            
            
            SemaphoreSlim limiter = new SemaphoreSlim(16);
            FeatureData?[] dtov2 = (await Task.WhenAll(
                wavTry2.Select(async (wavByte) =>
                {
                    await limiter.WaitAsync();
                    try
                    {
                        return await _myApi.GetFeaturesAsync("features", wavByte);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping {wavByte.Title}: {ex.Message}");
                        return null;
                    }
                    finally
                    {
                        limiter.Release();
                    }
                })
            )).Where(b => b != null).ToArray()!;
            sw.Stop();
            Console.WriteLine($"Genre Search Singles Elapsed: {sw.Elapsed.TotalSeconds:F3} seconds");
            Console.WriteLine(dtov2.Length);
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(dtov2[i]);
            }
            
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