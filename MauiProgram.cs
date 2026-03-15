using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProjectHellsParadise.BusinessLogic.APIs;

namespace ProjectHellsParadise;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Inter.tff", "Inter");
            });
        
        builder.Services.AddSingleton<DeezerClient>();
        builder.Services.AddSingleton<FeatureExtractionApi>();
        builder.Services.AddSingleton<SpotifyClient>();
        builder.Services.AddSingleton<AppleMusic>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}