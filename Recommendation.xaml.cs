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

    public Recommendation(DeezerClient deezerClient, FeatureExtractionApi extractionApi)
    {
        BindingContext = this;
        InitializeComponent();
        _isLoading = false;
        _wavEvent = new WaveOutEvent();
        _deezerClient = deezerClient;
        _myApi = extractionApi;
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

    private async void InitializePage()
    {
        IsLoading = true;
        PlayAudio(_featureData.SongBytes);
        await StartSearchViaGenre();
        IsLoading = false;
    }

    private async Task StartSearchViaGenre()
    {
        try
        {
            DeezerDTO trackData = await _deezerClient.GetGenreSongsAsync(_featureData.Genre[0].label);
            List<FeatureData> featureDataList = new List<FeatureData>() { _featureData };
            ByteRecord[] wavBytes = await _deezerClient.DownloadPreviewBytes(trackData);
            SemaphoreSlim limiter = new SemaphoreSlim(16);
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
            featureDataList.AddRange(featureDataArray);
            Vector vectorMaker = new Vector(featureDataList);
            float[][] vectors = vectorMaker.MakeVectors();
            for (int i = 0; i < vectors.Length; i++)
            {
                featureDataList[i].Vector = vectors[i];
            }
            List<SongSimilarity> newData = Vector.Rank(featureDataList);
            Console.WriteLine("Base Song: " + featureDataList[0].SongName);
            newData.ForEach(Console.WriteLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    

private void PlayAudio(byte[] wavBytes)
    {
        try
        {
            _wavEvent.Stop();
            MemoryStream memoryStream = new MemoryStream(wavBytes);
            memoryStream.Position = 0;
            WaveFileReader waveReader = new WaveFileReader(memoryStream);
            _wavEvent.Init(waveReader);
            _wavEvent.Volume = 0.01f;
            _wavEvent.Play();
        }
        catch (Exception)
        {
            throw new AudioPlayException("Failed to play audio");
        }
        
    }
    
    
}