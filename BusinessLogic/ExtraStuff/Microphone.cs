using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;

namespace ProjectHellsParadise.BusinessLogic.ExtraStuff;

public class Microphone
{
    private ISpeechToText _speechToText;
    
    public Microphone()
    {
        _speechToText = new SpeechToTextImplementation();
    }

    private string? RecognitionText {get; set;}
    
    public async Task<bool> ArePermissionsGranted()
    {
        PermissionStatus microphonePermissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
        bool isSpeechToTextRequestPermissionsGranted = await _speechToText.RequestPermissions(CancellationToken.None);

        return microphonePermissionStatus is PermissionStatus.Granted
               && isSpeechToTextRequestPermissionsGranted;
    }
    
    public async Task StartListening(CancellationToken cancellationToken)
    {
        bool isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission not granted").Show(CancellationToken.None);
            return;
        }

        _speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
        await _speechToText.StartListenAsync(new SpeechToTextOptions	{ Culture = CultureInfo.CurrentCulture, ShouldReportPartialResults = true }, CancellationToken.None);
    }

    public async Task StopListening(CancellationToken cancellationToken)
    {
        await _speechToText.StopListenAsync(CancellationToken.None);
        _speechToText.RecognitionResultUpdated -= OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted -= OnRecognitionTextCompleted;
    }

    void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
    {
        RecognitionText += args.RecognitionResult;
    }

    void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
    {
        RecognitionText = args.RecognitionResult.Text;
    }
    
    
    
}