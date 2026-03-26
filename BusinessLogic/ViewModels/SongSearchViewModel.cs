using System.Collections.ObjectModel;
using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;
using ProjectHellsParadise.BusinessLogic.ExtraStuff;
using ProjectHellsParadise.BusinessLogic.Models;
using ProjectHellsParadise.BusinessLogic.Services;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class SongSearchViewModel : ObservableObject
{
    private readonly FeatureExtractionApi _myApi;
    private readonly DeezerClient _deezerClient;
    private FeatureData _analyzedSong;
    private SongSessionService _songSessionService; 
    private readonly IDialogService _dialogService;
    private readonly AnalyticsClient _analyticsClient;
    private readonly CurrentUser _currentUser;
        
    [ObservableProperty]
    private ObservableCollection<DeezerDTO.DeezerData> _observableCollection;

    public SongSearchViewModel(DeezerClient deezerClient, FeatureExtractionApi extractionApi, SongSessionService songSessionService, IDialogService dialogService, AnalyticsClient analyticsClient, CurrentUser currentUser)
    {
        _analyzedSong = new FeatureData();
        _deezerClient = deezerClient;
        _myApi = extractionApi;
        _observableCollection = new ObservableCollection<DeezerDTO.DeezerData>();
        _songSessionService = songSessionService;
        _dialogService = dialogService;
        _analyticsClient = analyticsClient;
        _currentUser = currentUser;
    }
    
    [RelayCommand]
    private async Task SongSearch(string songName)
    {
        try{
            DeezerDTO deezerDto = await _deezerClient.GetAsync<DeezerDTO>("/search?q=", songName);
            ObservableCollection.Clear();
            foreach (DeezerDTO.DeezerData trackData in deezerDto.Data)
            {
                ObservableCollection.Add(trackData);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _dialogService.ShowAlertAsync("Error Finding Song", "An error has occured finding that song", "ok");
        }
    }

    [RelayCommand]
    private async Task SearchBarResultsChanged(DeezerDTO.DeezerData selected)
    {
        try
        {
            ByteRecord selectedSong = await _deezerClient.DownloadPreviewBytes(selected.Preview, selected.Title, selected.Artist.Name);
            _analyzedSong = await _myApi.GetFeaturesAsync("features", selectedSong);
            GenrePredictionDTO[] genreDto = await _myApi.PostAsync<GenrePredictionDTO[]>("classify", _analyzedSong.WavSongBytes);
            _analyzedSong.Genre = genreDto;
            
            _songSessionService.BaseSong = _analyzedSong;
            await _analyticsClient.IngestEvent("Selected Base Song", _currentUser.Id, properties: new Dictionary<string, object>
            {
                {"Song Name", _analyzedSong.SongName },
                {"User Name",  _analyzedSong.Artist},
                {"Genres", _analyzedSong.Genre.Select(g => g.label).ToList()}
            });
            await Shell.Current.GoToAsync(nameof(Recommendation));
        }
        catch (AudioPlayException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + ex.Source);
        }
    }
    
    [RelayCommand]
    private async Task SettingsPage() => await Shell.Current.GoToAsync(nameof(SettingsPage));
}