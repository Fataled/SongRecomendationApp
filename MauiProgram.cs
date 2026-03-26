using AnalyticsPipeline;
using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.ExtraStuff;
using ProjectHellsParadise.BusinessLogic.Services;
using ProjectHellsParadise.BusinessLogic.ViewModels;
using SkiaSharp.Views.Maui.Controls.Hosting;
using MauiIcons.Material;

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
            .UseMaterialMauiIcons()
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
        
        AuthClient.AuthClient authClient = new AuthClient.AuthClient("http://159.203.18.252.nip.io:8002");
        builder.Services.AddSingleton(authClient);
        
        Task.Run(async () => await
            authClient.RegisterOidcProviderAsync(
                "REMOVED_AUTH_TOKEN",
                "google",
                "REMOVED_GOOGLE_CLIENT_ID",
                "REMOVED_GOOGLE_CLIENT_SECRET",
                "https://accounts.google.com/.well-known/openid-configuration"
            ));
        
        Task.Run(async () => await
            authClient.ModifyMailService(
                "REMOVED_AUTH_TOKEN",
                "akobrume@gmail.com",
                "REMOVED_SENDGRID_API_KEY"
            ));
        
        builder.Services.AddSingleton<AnalyticsClient>(sp => new AnalyticsClient("http://159.203.18.252:8001"));
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<CurrentUser>();
        builder.Services.AddSingleton<RegisterPageViewModel>();
        builder.Services.AddSingleton<SettingsPageViewModel>();
        
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<AccountCreationPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RecommendationViewModel>();
        builder.Services.AddTransient<Recommendation>(); 
        builder.Services.AddTransient<AnalysisViewModel>();
        builder.Services.AddTransient<AnalysisPage>();
        builder.Services.AddTransient<SongSearchViewModel>();
        builder.Services.AddTransient<SongSearchPage>();
        builder.Services.AddTransient<SettingsPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}