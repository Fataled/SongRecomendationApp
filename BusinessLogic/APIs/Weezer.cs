using ProjectHellsParadise.BusinessLogic.Exceptions;

namespace ProjectHellsParadise.BusinessLogic.APIs;

using NAudio.Wave;
public class WeezerClient : ApiClientBase
{
    public WeezerClient() : base("https://api.deezer.com")
    {
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    public async Task<T> GetAsync<T>(string endpoint, string query)
    {
        return await RequestAsync<T>(endpoint, query);
    }

    protected override Task AddRequestHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    protected override Task AddContent(HttpRequestMessage requestMessage, object body)
    {
        return Task.CompletedTask;
    }

    public async Task<byte[]> DownloadPreviewBtyes(string previewUrl)
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
        catch (Exception ex)
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