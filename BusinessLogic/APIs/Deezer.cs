using System.Security.Cryptography;
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

public DeezerClient() : base("https://api.deezer.com") //TODO MORE GENRES AT A TIME
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
        const int playlistsToFetch = 5;                                    // fixed playlists per genre
        int songsPerPlaylist = Math.Max(1, 20 / genrePredictions.Length); // songs split across genres
        int remainder = 20 % genrePredictions.Length;
        Console.WriteLine("Genres: " + string.Join(", ", genrePredictions.Select(g => g.label)) );
        Random rng = new Random();

        try
        {
            foreach (var (prediction, index) in genrePredictions.Select((p, i) => (p, i)))
            {
                if (!GenreIds.ContainsKey(prediction.label))
                    continue;

                int songsToTake = songsPerPlaylist + (index < remainder ? 1 : 0); // distribute remainder

                DeezerDTO dto = await RequestAsync<DeezerDTO>(
                    "chart", 
                    GenreIds[prediction.label].ToString(), 
                    $"playlists?limit=100"
                );
                
                List<DeezerDTO.DeezerData> selectedPlaylists = dto.Data
                    .OrderBy(_ => rng.Next())
                    .Take(playlistsToFetch)
                    .ToList();
                

                foreach (DeezerDTO.DeezerData data in selectedPlaylists)
                {
                    DeezerDTO response = await RequestAsync<DeezerDTO>(
                        "playlist", 
                        data.Id.ToString(), 
                        "tracks?limit=100"
                    );

                    List<DeezerDTO.DeezerData> randomTracks = response.Data
                        .Where(t => !string.IsNullOrEmpty(t.Preview))
                        .OrderBy(_ => rng.Next())
                        .Take(songsToTake)
                        .ToList();

                    result.Data.AddRange(randomTracks);
                }
            }

            result.Data = result.Data.DistinctBy(t => t.Id).ToList();
            return result;
        }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException("No genre found: " + string.Join(", ", genrePredictions.Select(g => g.label)));
        }
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
            byte[] wavBytes = ConvertMp3BytesToWav(mp3Bytes);
            return new ByteRecord(wavBytes, title, artist);
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
    
    public async Task<ByteRecord[]> DownloadPreviewBytes(DeezerDTO dto)
    {
        try
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(50);
            var tasks = dto.Data.Select(async deezerData =>
                {
                    await semaphoreSlim.WaitAsync();
                    try
                    {
                        byte[] data = await HttpClient.GetByteArrayAsync(deezerData.Preview);
                        byte[] wavBytes = ConvertMp3BytesToWav(data);
                        return new ByteRecord(wavBytes, deezerData.Title, deezerData.Artist.Name);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }
            );

            var recordArray = await Task.WhenAll(tasks);
        
            return recordArray;
        }
        catch (ByteTransformationException ex)
        {
            throw new ByteTransformationException("Btye Transform Issue: " + ex.Message, ex);  
        }
        catch (Exception)
        {
            throw new ByteTransformationException("An error occured song previewUrl couldn't identify specific one");
        }
    }

    private byte[] ConvertMp3BytesToWav(byte[] mp3Bytes) 
    {
        try
        {
            MemoryStream mp3Stream = new MemoryStream(mp3Bytes);
            Mp3FileReader mp3FileReader = new Mp3FileReader(mp3Stream);

            WaveFormat outFormat = new WaveFormat(16000, 1);

            //THIS IS ON WINDOWS ONLY APPARENTLY
            MediaFoundationResampler resampler = new MediaFoundationResampler(mp3FileReader, outFormat);
            resampler.ResamplerQuality = 60;

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, resampler);
            return wavStream.ToArray();
        }
        catch
        {
            throw new ByteTransformationException("An error occured while converting the MP3 file to wav");
        }
    }
}