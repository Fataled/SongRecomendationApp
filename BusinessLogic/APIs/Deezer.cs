using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;

namespace ProjectHellsParadise.BusinessLogic.APIs;

using NAudio.Wave;

public class DeezerClient : ApiClientBase
{

    private static readonly Dictionary<string, int> GenreIds = new Dictionary<string, int>()
    {
        { "pop", 132 },
        { "hip-hop", 116 },
        { "rap", 116 },
        {"rock", 152},
        {"dance", 113},
        {"r&b", 165},
        {"alternative", 85},
        {"electro", 106},
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

public DeezerClient() : base("https://api.deezer.com")
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
            DeezerDTO dto = await RequestAsync<DeezerDTO>("chart", GenreIds[query].ToString(), "playlists");
            foreach (DeezerDTO.DeezerData data in dto.Data)
            {
                DeezerDTO response = await RequestAsync<DeezerDTO>("playlist", data.Id.ToString(), "tracks?limit=50");
                result.Data.AddRange(response.Data.Where(data => !string.IsNullOrEmpty(data.Preview)));
            }
            return result;
        }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException("No genre found for query: " + query);
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

    public async Task<byte[]> DownloadPreviewBytes(string previewUrl)
    {
        try
        {
            byte[] mp3Bytes = await HttpClient.GetByteArrayAsync(previewUrl);
            byte[] wavBytes = ConvertMp3BytesToWav(mp3Bytes);
            return wavBytes;
        }
        catch (ByteTransformationException ex)
        {
          throw new ByteTransformationException(ex.Message, ex);  
        }
        catch (Exception)
        {
            throw new ByteTransformationException("An error occured song previewUrl was: " + previewUrl);
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