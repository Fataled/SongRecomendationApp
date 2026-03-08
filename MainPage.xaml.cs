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

        FeatureExtractionApi myApi = await FeatureExtractionApi.CreateAsync();
        object data = await File.ReadAllBytesAsync(@"C:\Users\akobr\RiderProjects\ProjectHellsParadise\BusinessLogic\TestFiles\testFile.wav");
        try
        {
            FeatureExtractionDTO dto = await myApi.PostAsync<FeatureExtractionDTO>("/features", data);
            Console.WriteLine(dto.Embedding[0]);
        }
        catch(Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
}