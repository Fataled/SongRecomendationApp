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

    public Recommendation(DeezerClient deezerClient, FeatureExtractionApi extractionApi)
    {
        BindingContext = this;
        InitializeComponent();
        _isLoading = false;
        _wavEvent = new WaveOutEvent();
        _deezerClient = deezerClient;
        _myApi = extractionApi;
        _similarities = new ObservableCollection<SongSimilarity>();
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
        _wavEvent.Stop();
        _wavEvent.Dispose();
    }

    private async void InitializePage()
    {
        IsLoading = true;
        PlayAudio(_featureData.MP3SongBytes);
        await StartSearchViaGenre();
        IsLoading = false;
    }

   

    private async Task StartSearchViaGenre() //TODO ask if they wanna try again if not at least 5 above strong similarity
    {
        Stopwatch sw = new Stopwatch();
        try
        {
            sw.Start();
            DeezerDTO trackData = await _deezerClient.GetGenreSongsAsync(_featureData.GetMainGenres());
            Console.WriteLine($"Genre Elapsed: {sw.Elapsed.TotalSeconds:F3} seconds");
            sw.Stop();
            sw.Restart();
            List<FeatureData> featureDataList = [_featureData];
            sw.Start();
            ByteRecord[] wavBytes = await _deezerClient.DownloadPreviewBytes(trackData);
            Console.WriteLine($"Bytes Elapsed: {sw.Elapsed.TotalSeconds:F3} seconds");
            sw.Stop();
            sw.Restart();
            SemaphoreSlim limiter = new SemaphoreSlim(16);
            sw.Start();
            FeatureData?[] featureDataArray = (await Task.WhenAll(
                wavBytes.Select(async (wavByte) =>
                {
                    await limiter.WaitAsync();
                    try
                    {
                        return await _myApi.GetFeaturesAsync("features", wavByte);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping {wavByte.Title}: {ex.Message}");
                        return null;
                    }
                    finally
                    {
                        limiter.Release();
                    }
                })
            )).Where(b => b != null).ToArray();
            sw.Stop();
            Console.WriteLine($"Feature Elapsed: {sw.Elapsed.TotalSeconds:F3} seconds");
            sw.Restart();
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