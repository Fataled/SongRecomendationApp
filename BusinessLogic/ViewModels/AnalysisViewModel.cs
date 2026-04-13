using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Models;
using ProjectHellsParadise.BusinessLogic.Services;
using SkiaSharp;
using System.Text.Json;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class AnalysisViewModel : ObservableObject
{
    private readonly SongSessionService sessionService;
    private readonly AnalyticsClient analyticsClient;
    private readonly CurrentUser currentUser;

    [ObservableProperty]
    private ISeries[] series = Array.Empty<ISeries>();

    [ObservableProperty]
    private string ranking = string.Empty;

    [ObservableProperty]
    private string currentSongTitle = string.Empty;

    [ObservableProperty]
    private string currentArtistName = string.Empty;

    [ObservableProperty]
    private string currentSongStatus = "Choose Spotify or Apple Music.";

    [ObservableProperty]
    private bool isLaunchingPlatform;

    [ObservableProperty]
    private bool isGraphTabVisible = true;

    [ObservableProperty]
    private bool isMusicTabVisible = false;

    public PolarAxis[] AngleAxes { get; set; } =
    [
        new PolarAxis
        {
            Labels = new[]
            {
                "BPM", "Beats Confidence", "Key", "Key Strength", "Loudness",
                "Spectral Centroid", "Scale", "Danceability", "Dynamic Complexity",
                "Vocal", "MFCC 1", "MFCC 2", "MFCC 3", "MFCC 4", "MFCC 5",
                "MFCC 6", "MFCC 7", "MFCC 8", "MFCC 9", "MFCC 10", "MFCC 11", "MFCC 12", ""
            },
            MinStep = 1,
            ForceStepToMin = true,
            TextSize = 12
        }
    ];

    public AnalysisViewModel(
        SongSessionService songSessionService,
        AnalyticsClient analyticsClient,
        CurrentUser currentUser)
    {
        sessionService = songSessionService;
        this.analyticsClient = analyticsClient;
        this.currentUser = currentUser;

        Ranking = sessionService.SelectedSong?.Explanation ?? "No explanation available.";
        CurrentSongTitle = sessionService.SelectedSong?.Index
                           ?? sessionService.BaseSong?.SongName
                           ?? "Unknown song";
        CurrentArtistName = sessionService.BaseSong?.Artist ?? "Unknown artist";

        UpdatePolarChart();
    }

    private async void UpdatePolarChart()
    {
        if (sessionService.BaseSong?.Vector == null || sessionService.SelectedSong?.Vector == null)
        {
            Ranking = "No graph data available.";
            return;
        }

        Series =
        [
            new PolarLineSeries<float>
            {
                Name = sessionService.BaseSong.SongName,
                Values = sessionService.BaseSong.Vector,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0.2,
                IsClosed = true
            },
            new PolarLineSeries<float>
            {
                Name = sessionService.SelectedSong.Index,
                Values = sessionService.SelectedSong.Vector,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0.2,
                Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 2 },
                IsClosed = true
            }
        ];

        await analyticsClient.IngestEvent(
            "Viewed Graph",
            currentUser.Id,
            properties: new Dictionary<string, object>
            {
                { "Base Song", sessionService.BaseSong.SongName ?? "Unknown" },
                { "Selected Song", sessionService.SelectedSong.Index ?? "Unknown" },
                { "Similarity Score", sessionService.SelectedSong.Score }
            });
    }

    [RelayCommand]
    private void ShowGraphTab()
    {
        IsGraphTabVisible = true;
        IsMusicTabVisible = false;
    }

    [RelayCommand]
    private void ShowMusicTab()
    {
        IsGraphTabVisible = false;
        IsMusicTabVisible = true;
    }

    [RelayCommand]
    private async Task PlayOnSpotify()
    {
        await OpenSpotifyAsync();
    }

    [RelayCommand]
    private async Task PlayOnAppleMusic()
    {
        await OpenAppleMusicAsync();
    }

    private async Task OpenSpotifyAsync()
    {
        try
        {
            IsLaunchingPlatform = true;

            string query = $"{CurrentSongTitle} {CurrentArtistName}".Trim();
            string spotifySearchUrl = $"https://open.spotify.com/search/{Uri.EscapeDataString(query)}";

            await Launcher.Default.OpenAsync(spotifySearchUrl);
            CurrentSongStatus = $"Opened Spotify search for {CurrentSongTitle}.";
        }
        catch (Exception ex)
        {
            CurrentSongStatus = $"Spotify open failed: {ex.Message}";
        }
        finally
        {
            IsLaunchingPlatform = false;
        }
    }

    private async Task OpenAppleMusicAsync()
    {
        try
        {
            IsLaunchingPlatform = true;

            string query = $"{CurrentSongTitle} {CurrentArtistName}".Trim();
            string url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(query)}&entity=song&limit=1";

            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("results", out JsonElement results) &&
                results.GetArrayLength() > 0 &&
                results[0].TryGetProperty("trackViewUrl", out JsonElement trackViewUrl))
            {
                string? openUrl = trackViewUrl.GetString();

                if (!string.IsNullOrWhiteSpace(openUrl))
                {
                    await Launcher.Default.OpenAsync(openUrl);
                    CurrentSongStatus = $"Opened Apple Music for {CurrentSongTitle}.";
                    return;
                }
            }

            string fallback = $"https://music.apple.com/us/search?term={Uri.EscapeDataString(query)}";
            await Launcher.Default.OpenAsync(fallback);
            CurrentSongStatus = $"Opened Apple Music search for {CurrentSongTitle}.";
        }
        catch (Exception ex)
        {
            CurrentSongStatus = $"Apple Music open failed: {ex.Message}";
        }
        finally
        {
            IsLaunchingPlatform = false;
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}