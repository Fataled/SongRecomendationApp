using System.Net.Http.Headers;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;
using AnalyticsPipeline;

namespace ProjectHellsParadise.BusinessLogic.APIs;
/// <summary>
/// My own personal API that uses python + essentia to return features about songs
/// </summary>
/// <author>Brume Ako</author>
public class FeatureExtractionApi : ApiClientBase
{
    /// <summary>
    /// A bool to check if we are connected to the service
    /// </summary>
    private bool _connected;
    
    public FeatureExtractionApi() : base("http://159.203.18.252:8000") //  SERVER: http://159.203.18.252:4000  HOME: http://127.0.0.1:8000
    {
        _connected = false; //TODO make it so on shutdown this becomes false or on disconnect So far we have smth but not sure if its the best idea
        HttpClient.Timeout = TimeSpan.FromSeconds(600);
    }
    
    /// <summary>
    /// Make sure the server is online before attempting to send data
    /// </summary>
    private async Task WaitForServer()
    {
        if (_connected) return;
        
        const int maxAttempts = 10;
        const int delayMs = 1000;

        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                HttpResponseMessage responseMessage = await HttpClient.GetAsync("/");
                if (responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Connection established successfully.");
                    _connected = true;
                    return;
                }
            }
            catch
            {
                
            }
            await Task.Delay(delayMs);
        }
    }
    
    /// <summary>
    ///  Asks the server for the features of a song based of a clip sent
    /// </summary>
    /// <param name="endpoint">The endpoint used typically /features</param>
    /// <param name="body">A byterecord which contains the songs name, mp3 and artist name</param>
    /// <returns>A new FeatureData object that contains the title, artist name, the entire dto and both byte arrays</returns>
    public async Task<FeatureData> GetFeaturesAsync(string endpoint, object body)
    {
        await ReInitializeConnection();
        ByteRecord byteRecord = (ByteRecord)body;
        byte[] wavBytes = await Task.Run(() => DeezerClient.ConvertMp3BytesToWav(byteRecord.Mp3Bytes));
        FeatureExtractionDTO dto = await SendAsync<FeatureExtractionDTO>(endpoint, wavBytes);
        return new FeatureData(byteRecord.Title, byteRecord.Artist, dto, wavBytes, byteRecord.Mp3Bytes);
    }
    
    /// <summary>
    /// The regular POST method used for single one off calls to endpoints like /classify or /
    /// </summary>
    /// <param name="endpoint">The endpoint you want to talk to</param>
    /// <param name="body">The body being sent</param>
    /// <typeparam name="T">The type to parse the JSON to</typeparam>
    /// <returns>A new object in T's type</returns>
    public async Task<T> PostAsync<T>(string endpoint, object body)
    {
        await ReInitializeConnection();
        return await SendAsync<T>(endpoint, body);
    }

    protected override Task AddAuthHeader(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Add the request header of audio/wav if needed
    /// </summary>
    /// <param name="request">The HttpRequestMessge being used</param>
    /// <returns>That the task is done and modifies the request</returns>
    protected override Task AddRequestHeader(HttpRequestMessage request)
    {
        if (request.Content is not MultipartFormDataContent)
        {
            request.Content?.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        }
        return Task.CompletedTask;
    }
    /// <summary>
    /// Add the content to the request
    /// </summary>
    /// <param name="request">The HttpRequestMessge being used</param>
    /// <param name="body">The data that you are trying to send</param>
    /// <returns>Task is completed and modifies the request</returns>
    protected override Task AddContent(HttpRequestMessage request, object body)
    {
        switch (body)
        {
            case byte[][] wavFiles:
                MultipartFormDataContent form = new MultipartFormDataContent();
                foreach (byte[] wavByte in wavFiles)
                {
                    ByteArrayContent fileContent = new ByteArrayContent(wavByte);
                    form.Add(fileContent, "wav_data", "audio.wav");
                }
                request.Content = form;
                break;
            default:
                request.Content = new ByteArrayContent((byte[])body);
                break;
        }
        return Task.CompletedTask;
    }
    /// <summary>
    /// If the server isn't connected try and connect
    /// </summary>
    private async Task ReInitializeConnection()
    {
        if (_connected) return;
        await WaitForServer();
    }
}