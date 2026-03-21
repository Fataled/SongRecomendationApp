using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.ExtraStuff;
using ProjectHellsParadise.BusinessLogic.Services;
using ProjectHellsParadise.BusinessLogic.ViewModels;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace ProjectHellsParadise;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseSkiaSharp()
            .UseLiveCharts()
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
        builder.Services.AddSingleton<SongSessionService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();

        builder.Services.AddTransient<RecommendationViewModel>();
        builder.Services.AddTransient<Recommendation>(); 
        builder.Services.AddTransient<AnalysisViewModel>();
        builder.Services.AddTransient<AnalysisPage>();
        builder.Services.AddTransient<SongSearchViewModel>();
        builder.Services.AddTransient<SongSearchPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}