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
        DeezerDTO trackData = await weezerClient.GetAsync<DeezerDTO>("/search?q=", "Cozy Forever KembeX");
        try
        {
            byte[] wavBtyes = await weezerClient.DownloadPreviewBtyes(trackData.Data[0].Preview);

            FeatureExtractionApi myApi = new FeatureExtractionApi();

            FeatureExtractionDTO dto = await myApi.PostAsync<FeatureExtractionDTO>("/features", wavBtyes);
            Console.WriteLine(dto.Embedding[0]);
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