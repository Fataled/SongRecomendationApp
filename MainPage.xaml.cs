using System.Diagnostics;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        DeezerClient weezerClient = new DeezerClient();
        DeezerDTO trackData = await weezerClient.GetGenreSongsAsync("rap");
        try
        {
            ByteRecord[] wavBytes = (await Task.WhenAll(
                trackData.Data.Select(async data =>
                {
                    try
                    {
                        return await weezerClient.DownloadPreviewBytes(data.Preview, data.Title);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping {data.Title}: {ex.Message}");
                        return null;
                    }
                })
            )).Where(b => b != null).ToArray()!;

            FeatureExtractionApi myApi = new FeatureExtractionApi();
            
            
            
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                FeatureExtractionDTO[] dto = await myApi.GetFeaturesAsync<FeatureExtractionDTO[]>("features/batch", wavBytes.Select(data => data.PreviewBytes).ToArray());
                stopwatch.Stop();
                Console.WriteLine($"Genre Search Batch Elapsed: {stopwatch.Elapsed.TotalSeconds:F3} seconds");
                Console.WriteLine(dto.Length);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Stopwatch sw = Stopwatch.StartNew();
            FeatureExtractionDTO?[] dtov2 = (await Task.WhenAll(
                wavBytes.Select(async (wavByte) =>
                {
                    try
                    {
                        return await myApi.GetFeaturesAsync<FeatureExtractionDTO>("/features", wavByte.PreviewBytes);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping {wavByte.Title}: {ex.Message}");
                        return null;
                    }
                })
            )).Where(b => b != null).ToArray()!;
            sw.Stop();
            Console.WriteLine($"Genre Search Singles Elapsed: {sw.Elapsed.TotalSeconds:F3} seconds");
            Console.WriteLine(dtov2.Length);
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