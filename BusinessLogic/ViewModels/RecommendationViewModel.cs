using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AnalyticsPipeline;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.Wave;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;
using ProjectHellsParadise.BusinessLogic.Models;
using ProjectHellsParadise.BusinessLogic.MyMath;
using ProjectHellsParadise.BusinessLogic.Services;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;
public partial class RecommendationViewModel : ObservableObject {
    
    private WaveOutEvent _wavEvent;
    private CancellationTokenSource _cancellationToken;
    private readonly FeatureExtractionApi _myApi;
    private DeezerClient _deezerClient;
    private SongSessionService  _songSessionService;
    private AnalyticsClient _analyticsClient;
    private readonly CurrentUser _currentUser;
    
    [ObservableProperty]
    FeatureData _featureData;
        
    [ObservableProperty]
    ObservableCollection<SongSimilarity> _similarities;

    [ObservableProperty]
    private bool isLoading;

    
    public RecommendationViewModel(FeatureExtractionApi extractionApi, DeezerClient deezerClient, SongSessionService songSessionService, AnalyticsClient analyticsClient, CurrentUser currentUser)
    {
        _wavEvent = new WaveOutEvent();
        _deezerClient = deezerClient;
        _myApi = extractionApi;
        _similarities = new ObservableCollection<SongSimilarity>();
        _cancellationToken = new CancellationTokenSource();
        _songSessionService = songSessionService;
        _analyticsClient = analyticsClient;
        _currentUser = currentUser;
        InitAsync().FireAndForget();
    }

    private async Task InitAsync()
    {
        IsLoading = true;
        FeatureData = _songSessionService.BaseSong;
        PlayAudio(FeatureData.MP3SongBytes);
        await StartSearchViaGenre(_cancellationToken);
        IsLoading = false;
    }
    
    private async Task StartSearchViaGenre(CancellationTokenSource token) //TODO ask if they wanna try again if not at least 5 above strong similarity
    {
        Stopwatch sw = new Stopwatch();
        Console.WriteLine(FeatureData);
        try
        {
            DeezerDTO trackData = await _deezerClient.GetGenreSongsAsync(FeatureData.GetMainGenres());
            List<FeatureData> featureDataList = [FeatureData];
            SemaphoreSlim limiter = new SemaphoreSlim(16);
            ConcurrentBag<FeatureData> results = new();
            List<Task> featureTasks = new();
            sw.Start();
            await foreach (ByteRecord record in _deezerClient.DownloadPreviewBytesStreamed(trackData))
            {
                featureTasks.Add(Task.Run(async () =>
                {
                    await limiter.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        FeatureData data = await _myApi.GetFeaturesAsync("features", record);
                        results.Add(data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping {record.Title}: {ex.Message}");
                    }
                    finally
                    {
                        limiter.Release();
                    }
                }));
            }
            await Task.WhenAll(featureTasks);
            sw.Stop();
            await _analyticsClient.IngestEvent("Genre Search", _currentUser.Id, properties: new Dictionary<string, object>
            {
                { "Songs Found", results.Count},
                {"Time Take", sw.Elapsed}
            });
            
            FeatureData[] featureDataArray = results.ToArray();
            featureDataList.AddRange(featureDataArray);
            Vector vectorMaker = new Vector(featureDataList);
            float[][] vectors = vectorMaker.MakeVectors();
            for (int i = 0; i < vectors.Length; i++)
            {
                featureDataList[i].Vector = vectors[i];
            }
            List<SongSimilarity> newData = Vector.Rank(featureDataList);
            Similarities.Clear();
            foreach (SongSimilarity similarity in newData.Where(t => t.Score > 0.85))
            {
                Similarities.Add(similarity);
            }

            _songSessionService.Results = Similarities.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private void PlayAudio(byte[] mp3Bytes)
    {
        try
        {
            _wavEvent.Stop();
            _wavEvent.Dispose();
            _wavEvent = new WaveOutEvent();
            MemoryStream memoryStream = new MemoryStream(mp3Bytes);
            memoryStream.Position = 0;
            Mp3FileReader mp3Reader = new Mp3FileReader(memoryStream);
            _wavEvent.Init(mp3Reader);
            _wavEvent.Volume = 0.01f;
            _wavEvent.Play();
        }
        catch (Exception)
        {
            throw new AudioPlayException("Failed to play audio");
        }
    }

    [RelayCommand]
    private async Task SelectSong(SongSimilarity similarity)
    {
        if (similarity == null) return;
        _songSessionService.SelectedSong = similarity;
        Console.WriteLine(similarity);
        await Shell.Current.GoToAsync(nameof(AnalysisPage),true);
    }

    [RelayCommand]
    private async Task GoBack() => await Shell.Current.GoToAsync("..");
}