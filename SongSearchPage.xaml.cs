using System.Collections.ObjectModel;
using NAudio.Wave;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise;

public partial class SongSearchPage : ContentPage
{
    private DeezerClient _deezerClient;
    private FeatureExtractionApi _extractionApi;
    private ObservableCollection<DeezerDTO.DeezerTrack> _observableCollection;
    private byte[] _selectedSong;
    private FeatureData _analyzedSong;
    private WaveOutEvent _wavEvent;
    
    public SongSearchPage()
    {
        _analyzedSong = new FeatureData();
        _deezerClient = new DeezerClient();
        _extractionApi = new FeatureExtractionApi();
        _observableCollection = new ObservableCollection<DeezerDTO.DeezerTrack>();
        _wavEvent = new WaveOutEvent();
        _selectedSong = [];
        BindingContext = this;
        InitializeComponent();
        
    }
    
    public ObservableCollection<DeezerDTO.DeezerTrack> ObservableCollection => _observableCollection;

    private async void SongSearch(object? sender, EventArgs e)
    {
        try{
            SearchBar searchBar = (SearchBar)sender!;
            string songName = searchBar.Text;
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
            if (e.CurrentSelection.Count == 0) return;
            DeezerDTO.DeezerTrack trackData = (DeezerDTO.DeezerTrack)e.CurrentSelection[0];
            _selectedSong = await _deezerClient.DownloadPreviewBtyes(trackData.Preview);
            FeatureExtractionDTO extractionDto =
                await _extractionApi.PostAsync<FeatureExtractionDTO>("/features", _selectedSong);
            _analyzedSong = new FeatureData(trackData.Title, trackData.Artist.Name, extractionDto.Embedding);
            playAudio(_selectedSong);
        }
        catch (AudioPlayException ex)
        {
            Console.WriteLine(ex.Message);
            await DisplayAlertAsync("Player failure", "Failed to play audio", "ok");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await DisplayAlertAsync("Error Downloading Song", "An error has occured downloading that song", "ok");
        }
    }

    private void playAudio(byte[] wavBytes)
    {
        try
        {
            _wavEvent.Stop();
            MemoryStream memoryStream = new MemoryStream(wavBytes);
            memoryStream.Position = 0;
            WaveFileReader waveReader = new WaveFileReader(memoryStream);
            _wavEvent.Init(waveReader);
            _wavEvent.Play();
        }
        catch (Exception ex)
        {
            throw new AudioPlayException("Failed to play audio");
        }
        
    }
}