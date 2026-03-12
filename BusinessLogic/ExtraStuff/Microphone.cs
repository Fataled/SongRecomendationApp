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
        Boolean isSpeechToTextRequestPermissionsGranted = await _speechToText.RequestPermissions(CancellationToken.None);

        return microphonePermissionStatus is PermissionStatus.Granted
               && isSpeechToTextRequestPermissionsGranted;
    }
    
    async Task StartListening(CancellationToken cancellationToken)
    {
        var isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission not granted").Show(CancellationToken.None);
            return;
        }

        _speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
        await _speechToText.StartListenAsync(new SpeechToTextOptions	{ Culture = CultureInfo.CurrentCulture, ShouldReportPartialResults = true }, CancellationToken.None);
    }

    async Task StopListening(CancellationToken cancellationToken)
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