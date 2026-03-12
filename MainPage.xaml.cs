using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

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
            
            List<FeatureExtractionDTO> results = new List<FeatureExtractionDTO>();
            int batchSize = 20;

            for (int i = 0; i < wavBytes.Length; i += batchSize)
            {
                var batch = wavBytes.Skip(i).Take(batchSize).ToArray();
                var batchIndices = Enumerable.Range(i, batch.Length).ToArray();

                var batchResults = await Task.WhenAll(
                    batch.Select(async (wavByte, j) =>
                    {
                        try
                        {
                            return await myApi.GetFeaturesAsync<FeatureExtractionDTO>("/features", wavByte);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Skipping {trackData.Data[batchIndices[j]].Title}: {ex.Message}");
                            return null;
                        }
                    })
                );

                results.AddRange(batchResults.Where(b => b != null)!);
                Console.WriteLine($"Processed {Math.Min(i + batchSize, wavBytes.Length)}/{wavBytes.Length}");
            }

            FeatureExtractionDTO[] dto = results.ToArray();
            
            
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