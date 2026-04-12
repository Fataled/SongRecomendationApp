using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
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

    //Spotify dependencies(Obaid)
    private readonly SpotifyClient _spotifyClient;
    private readonly RandomSongService _randomSongService;
        
    [ObservableProperty]
    private ObservableCollection<DeezerDTO.DeezerData> _observableCollection;

    /// <summary>
    /// Bound to the Spotify CollectionView when IsSpotifyMode is true.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<SpotifyMusicSong> _spotifyResults;

    /// <summary>
    /// Loaded from the JSON  history file on startup and when spotify tab is activated
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<SpotifyHistoryService.RecentSongEntry> _recentlyOpened;

    /// <summary>
    /// True = Spotify Mode. False = Deezer Mode (default)
    /// </summary>
    [ObservableProperty]
    private bool _isSpotifyMode;

    /// <summary>
    /// Inverse of IsSpotifyMode for XAML IsVisible bindings on Deezer elements
    /// </summary>
    public bool IsDeezerMode => !IsSpotifyMode;

    /// <summary>
    /// True while the random song pipeline is running
    /// </summary>
    [ObservableProperty]
    private bool _isRandomLoading;
    
    public SongSearchViewModel(DeezerClient deezerClient, FeatureExtractionApi extractionApi, SongSessionService songSessionService, IDialogService dialogService, AnalyticsClient analyticsClient, CurrentUser currentUser, SpotifyClient spotifyClient, RandomSongService randomSongService)
    {
        _analyzedSong = new FeatureData();
        _deezerClient = deezerClient;
        _myApi = extractionApi;
        _observableCollection = new ObservableCollection<DeezerDTO.DeezerData>();
        _songSessionService = songSessionService;
        _dialogService = dialogService;
        _analyticsClient = analyticsClient;
        _currentUser = currentUser;
        _spotifyClient = spotifyClient;
        _randomSongService = randomSongService;
        _spotifyResults = new ObservableCollection<SpotifyMusicSong>();
        _recentlyOpened = new ObservableCollection<SpotifyHistoryService.RecentSongEntry>();
        _isSpotifyMode = false;
        _isRandomLoading = false;
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

    // Spotify commands (Obaid)

    [RelayCommand]
    private async Task ToggleSpotifyMode()
    {
        _isSpotifyMode = !_isSpotifyMode;
        OnPropertyChanged(nameof(IsSpotifyMode));
        OnPropertyChanged(nameof(IsDeezerMode));

        if (_isSpotifyMode)
            await LoadRecentlyOpenedAsync();
    }

    /// <summary>
    /// Searches spotify for tracks and populates SpotifyResults
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task SpotifySearch(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        try
        {
            await _spotifyClient.GetHistoryService().SaveSearchAsync(query);

            List<SpotifyMusicSong> results = await _spotifyClient.SearchTracksAsync(query, limit: 20);
            _spotifyResults.Clear();
            foreach (SpotifyMusicSong song in results)
                _spotifyResults.Add(song);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _dialogService.ShowAlertAsync("Spotify Error", "Could not search Spotify. Please try again.", "OK");
        }
    }

    /// <summary>
    /// called when the user taps a spotify search result
    /// opens the track in the spotify app or browser
    /// </summary>
    /// <param name="selected"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task SpotifyResultsSelected(SpotifyMusicSong selected)
    {
        if (selected == null) 
            return;

        try
        {
            await _spotifyClient.OpenInSpotifyAsync(selected);
            await LoadRecentlyOpenedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _dialogService.ShowAlertAsync("Spotify error.", "Could not open this track in spotify.", "OK");
        }
    }

    [RelayCommand]
    private async Task RecentSongSelected(SpotifyHistoryService.RecentSongEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.ExternalUrl))
            return;

        try
        {
            await Launcher.OpenAsync(new Uri(entry.ExternalUrl));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _dialogService.ShowAlertAsync("Error", "Could not open this track.", "OK");
        }
    }
    private async Task LoadRecentlyOpenedAsync()
    {
        try
        {
            List<SpotifyHistoryService.RecentSongEntry> history =
                await _spotifyClient.GetHistoryService().GetRecentlyOpenedAsync();

            _recentlyOpened.Clear();
            foreach (SpotifyHistoryService.RecentSongEntry entry in history)
                _recentlyOpened.Add(entry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SongSearchViewModel] Failed to load history: {ex.Message}");
        }
    }

    // Random Song Command (Obaid)

    [RelayCommand]
    private async Task RandomSong()
    {
        try
        {
            _isRandomLoading = true;
            OnPropertyChanged(nameof(IsRandomLoading));

            FeatureData randomSong = await _randomSongService.GetRandomSongAsync();
            _songSessionService.BaseSong = randomSong;

            await _analyticsClient.IngestEvent("Random Song Selected", _currentUser.Id, properties: new Dictionary<string, object>
            {
                {"Song Name", randomSong.SongName },
                {"Artist", randomSong.Artist },
                {"Genres", randomSong.Genre.Select(g => g.label).ToList() }
            });

            await Shell.Current.GoToAsync(nameof(Recommendation));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _dialogService.ShowAlertAsync("Random Song Error", "Could not find a random song. Please try again.", "OK");
        }
        finally
        {
            _isRandomLoading = false;
            OnPropertyChanged(nameof(IsRandomLoading));
        }
    }

    [RelayCommand]
    private async Task SettingsPage() => await Shell.Current.GoToAsync(nameof(SettingsPage));
}