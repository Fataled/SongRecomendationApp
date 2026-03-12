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
            byte[][] wavBytes = (await Task.WhenAll(
                trackData.Data.Select(async data =>
                {
                    try
                    {
                        return await weezerClient.DownloadPreviewBytes(data.Preview);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping {data.Title}: {ex.Message}");
                        return null;
                    }
                })
            )).Where(b => b != null).ToArray()!;

            FeatureExtractionApi myApi = new FeatureExtractionApi();

            FeatureData[] dto = (await Task.WhenAll(
                wavBytes.Select(async (wavByte, i) =>
                {
                    try
                    {
                        return await myApi.GetFeaturesAsync("/features", wavByte,  trackData.Data[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping a song: {ex.Message}");
                        return null;
                    }
                })
            )).Where(b => b != null).ToArray()!;;
            
            
            Console.WriteLine(dto.Length);
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