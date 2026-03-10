using System.Collections.ObjectModel;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise;

public partial class SongSearchPage : ContentPage
{
    private DeezerClient _deezerClient;
    private FeatureExtractionApi _extractionApi;
    private ObservableCollection<DeezerDTO.DeezerTrack> _observableCollection;
    private byte[] _selectedSong;
    private FeatureData _analyzedSong;
    
    public SongSearchPage()
    {
        _deezerClient = new DeezerClient();
        _extractionApi = new FeatureExtractionApi();
        _observableCollection = new ObservableCollection<DeezerDTO.DeezerTrack>();
        _selectedSong = [];
        BindingContext = this;
        InitializeComponent();
        
    }
    
    public ObservableCollection<DeezerDTO.DeezerTrack> ObservableCollection { get => _observableCollection; }
    private async void SongSearch(object? sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        string songName = searchBar.Text;
        try
        {
            DeezerDTO deezerDto = await _deezerClient.GetAsync<DeezerDTO>("/search?q=", songName);
            _observableCollection.Clear();
            foreach (DeezerDTO.DeezerTrack trackData in deezerDto.Data)
            {
                _observableCollection.Add(trackData);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            await DisplayAlertAsync("Error Finding Song", "An error has occured finding that song", "ok");
        }
    }

    private async void SearchBarResultsChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            DeezerDTO.DeezerTrack trackData = (DeezerDTO.DeezerTrack)e.CurrentSelection[0];
            _selectedSong = await _deezerClient.DownloadPreviewBtyes(trackData.Preview);
            FeatureExtractionDTO extractionDto = await _extractionApi.PostAsync<FeatureExtractionDTO>("/features", _selectedSong);
            _analyzedSong = new FeatureData(trackData.Title, trackData.Artist.Name, extractionDto.Embedding);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await DisplayAlertAsync("Error Downloading Song", "An error has occured downloading that song", "ok");
        }
    }
}