using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using NAudio.Wave;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;
using ProjectHellsParadise.BusinessLogic.MyMath;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise;

[QueryProperty(nameof(FeatureData), "_analyzedSong")]

public partial class Recommendation : ContentPage
{
    bool _isLoading;
    FeatureData _featureData;
    private WaveOutEvent _wavEvent;
    private DeezerClient _deezerClient;
    private FeatureExtractionApi _myApi;
    private ObservableCollection<SongSimilarity> _similarities;
    private CancellationTokenSource _cancellationToken;

    public Recommendation(DeezerClient deezerClient, FeatureExtractionApi extractionApi)
    {
        BindingContext = this;
        InitializeComponent();
        _isLoading = false;
        _wavEvent = new WaveOutEvent();
        _deezerClient = deezerClient;
        _myApi = extractionApi;
        _similarities = new ObservableCollection<SongSimilarity>();
        _cancellationToken = new CancellationTokenSource();
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public FeatureData FeatureData
    {
        get => _featureData;
        set
        {
            _featureData = value;
            OnPropertyChanged();
            InitializePage();
        }
    }

    public ObservableCollection<SongSimilarity> Similarities
    {
        get => _similarities;
        set {
            _similarities = value;
            OnPropertyChanged();
        }
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cancellationToken.Cancel();
        _wavEvent.Stop();
        _wavEvent.Dispose();
    }

    private async void InitializePage()
    {
        IsLoading = true;
        PlayAudio(_featureData.MP3SongBytes);
        await StartSearchViaGenre(_cancellationToken);
        IsLoading = false;
    }

    private async Task StartSearchViaGenre(CancellationTokenSource token) //TODO ask if they wanna try again if not at least 5 above strong similarity
    {
        Stopwatch sw = new Stopwatch();
        Console.WriteLine(_featureData);
        try
        {
            DeezerDTO trackData = await _deezerClient.GetGenreSongsAsync(_featureData.GetMainGenres());
            List<FeatureData> featureDataList = [_featureData];
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
                        FeatureData? data = await _myApi.GetFeaturesAsync("features", record);
                        if (data != null) results.Add(data);
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
            sw.Stop();
            Console.WriteLine($"{sw.Elapsed}");
            await Task.WhenAll(featureTasks);
            FeatureData[] featureDataArray = results.ToArray();
            featureDataList.AddRange(featureDataArray);
            Vector vectorMaker = new Vector(featureDataList);
            float[][] vectors = vectorMaker.MakeVectors();
            for (int i = 0; i < vectors.Length; i++)
            {
                featureDataList[i].Vector = vectors[i];
            }
            List<SongSimilarity> newData = Vector.Rank(featureDataList);
            _similarities.Clear();
            foreach (SongSimilarity similarity in newData.Where(t => t.Score > 0.85))
            {
                _similarities.Add(similarity);
            }
            _recomendationsView.ItemsSource = _similarities;
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

    private void RecomendationSelected(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection.Count == 0) return;
            SongSimilarity selectedSong = (SongSimilarity)e.CurrentSelection[0];
            PlayAudio(selectedSong.Mp3Bytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}