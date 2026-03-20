using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using NAudio.Wave.SampleProviders;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;

namespace ProjectHellsParadise.BusinessLogic.APIs;

using NAudio.Wave;

public class DeezerClient : ApiClientBase
{
    private static readonly Dictionary<string, int> GenreIds = new Dictionary<string, int>()
    {
        { "pop", 132 },
        { "hip hop", 116 },
        { "rap", 116 },
        {"rock", 152},
        {"dance", 113},
        {"r&b", 165},
        {"alternative", 85},
        {"electronic", 106},
        {"folk", 466},
        {"reggae", 144},
        {"jazz", 129},
        {"country", 84},
        {"french chanson", 52},
        {"classical", 98},
        {"films", 173},
        {"games", 173},
        {"metal", 464},
        {"soul", 169},
        {"african music", 2},
        {"asian music", 16},
        {"blues", 153},
        {"brazillian music", 75},
        {"indian music", 81},
        {"kids", 95},
        {"latin music", 197}
    };

public DeezerClient() : base("https://api.deezer.com", new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections =  true,
        MaxConnectionsPerServer = 100,
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
    })
    {

    }


    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    public async Task<T> GetAsync<T>(string endpoint, string query, string? ending = null)
    {
        if (GenreIds.TryGetValue(query, out int genreId)) return await RequestAsync<T>(endpoint, genreId.ToString(), ending);
        
        return await RequestAsync<T>(endpoint, query, ending);
    }

    public async Task<DeezerDTO> GetGenreSongsAsync(string query)
    {
        DeezerDTO result = new DeezerDTO();
        try
        {
            string genreName = query;
            DeezerDTO dto = await RequestAsync<DeezerDTO>("chart", GenreIds[genreName].ToString(), "playlists?limit=20"); //TODO more playlist variation
            foreach (DeezerDTO.DeezerData data in dto.Data)
            {
                DeezerDTO response = await RequestAsync<DeezerDTO>("playlist", data.Id.ToString(), "tracks?limit=10"); //TODO CHANGE TO 50
                result.Data.AddRange(response.Data.Where(deezerData => !string.IsNullOrEmpty(deezerData.Preview)));
            }
            return result;
        }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException("No genre found for query: " + query);
        }
    }
    
    public async Task<DeezerDTO> GetGenreSongsAsync(GenrePredictionDTO[] genrePredictions)
    {
        DeezerDTO result = new DeezerDTO();
        const int playlistsToFetch = 10;
        int songsPerPlaylist = Math.Max(1, 50 / genrePredictions.Length);
        int remainder = 50 % genrePredictions.Length;
        Random rng = new Random();

        IEnumerable<Task<List<DeezerDTO.DeezerData>>> genreTasks = genrePredictions.Select(async (prediction, index) =>
        {
            if (!GenreIds.ContainsKey(prediction.label)) return new List<DeezerDTO.DeezerData>();

            int songsToTake = songsPerPlaylist + (index < remainder ? 1 : 0);

            DeezerDTO dto = await RequestAsync<DeezerDTO>("chart", GenreIds[prediction.label].ToString(), "playlists?limit=100");

            List<DeezerDTO.DeezerData> selectedPlaylists = dto.Data.OrderBy(_ => rng.Next())
                .Take(playlistsToFetch)
                .ToList();

            IEnumerable<Task<List<DeezerDTO.DeezerData>>> playlistTasks = selectedPlaylists.Select(async data =>
            {
                DeezerDTO response = await RequestAsync<DeezerDTO>("playlist", data.Id.ToString(), "tracks?limit=100");

                return response.Data.Where(t => !string.IsNullOrEmpty(t.Preview))
                    .OrderBy(_ => rng.Next())
                    .Take(songsToTake)
                    .ToList();
            });

            List<DeezerDTO.DeezerData>[] playlistResults = await Task.WhenAll(playlistTasks);
            return playlistResults.SelectMany(x => x).ToList();
        });

        List<DeezerDTO.DeezerData>[] allResults = await Task.WhenAll(genreTasks);
        result.Data = allResults.SelectMany(x => x).DistinctBy(t => t.Id).ToList();
        return result;
    }

    protected override Task AddRequestHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    protected override Task AddContent(HttpRequestMessage requestMessage, object body)
    {
        return Task.CompletedTask;
    }

    public async Task<ByteRecord> DownloadPreviewBytes(string previewUrl, string title = "", string artist = "")
    {
        try
        {
            byte[] mp3Bytes = await HttpClient.GetByteArrayAsync(previewUrl);
            return new ByteRecord(title, artist, mp3Bytes);
        }
        catch (ByteTransformationException ex)
        {
          throw new ByteTransformationException("Btye Transform Issue: " + ex.Message, ex);  
        }
        catch (Exception)
        {
            throw new ByteTransformationException("An error occured song previewUrl was: " + previewUrl);
        }
    }
    
    public async IAsyncEnumerable<ByteRecord> DownloadPreviewBytesStreamed(
        DeezerDTO dto,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        SemaphoreSlim semaphore = new(20);
        ConcurrentQueue<ByteRecord> ready = new();

        List<Task> tasks = dto.Data.Select(async deezerData =>
        {
            await semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                using HttpResponseMessage response = await HttpClient
                    .GetAsync(deezerData.Preview, HttpCompletionOption.ResponseHeadersRead, ct)
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                byte[] mp3Bytes = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
                ready.Enqueue(new ByteRecord(deezerData.Title, deezerData.Artist.Name, mp3Bytes));
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        while (tasks.Any(t => !t.IsCompleted) || !ready.IsEmpty)
        {
            while (ready.TryDequeue(out ByteRecord? record))
                yield return record;

            await Task.Delay(50, ct).ConfigureAwait(false);
        }
    }
    
    public static byte[] ConvertMp3BytesToWav(byte[] mp3Bytes) 
    {
        try
        {
            using MemoryStream mp3Stream = new MemoryStream(mp3Bytes);
            using Mp3FileReader mp3FileReader = new Mp3FileReader(mp3Stream);
            
            WdlResamplingSampleProvider resampler = new WdlResamplingSampleProvider(mp3FileReader.ToSampleProvider().ToMono(), 16000);

            using MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, resampler.ToWaveProvider16());
            return wavStream.ToArray();
        }
        catch
        {
            throw new ByteTransformationException("An error occured while converting the MP3 file to wav");
        }
    }
}