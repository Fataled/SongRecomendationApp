using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ProjectHellsParadise.BusinessLogic.Services;
using SkiaSharp;

namespace ProjectHellsParadise.BusinessLogic.ViewModels;

public partial class AnalysisViewModel : ObservableObject
{
    private SongSessionService _sessionService;

    [ObservableProperty] private ISeries[] _series = Array.Empty<ISeries>();

    [ObservableProperty] private string _ranking;
    
    public PolarAxis[] AngleAxes { get; set; } = new PolarAxis[]
    {
        new PolarAxis
        {
            Labels = new[]
            {
                "BPM",
                "Beats Confidence",
                "Key",
                "Key Strength",
                "Loudness",
                "Spectral Centroid",
                "Scale",
                "Danceability",
                "Dynamic Complexity",
                "Vocal",
                "MFCC 1", "MFCC 2", "MFCC 3", "MFCC 4",
                "MFCC 5", "MFCC 6", "MFCC 7", "MFCC 8",
                "MFCC 9", "MFCC 10", "MFCC 11", "MFCC 12",
            },
            MinStep = 1,
            ForceStepToMin = true,
            TextSize = 12
        }
    };

    public AnalysisViewModel(SongSessionService songSessionService)
    {
        try
        {
            _sessionService = songSessionService;
            Ranking = _sessionService.SelectedSong.Explanation;
            UpdatePolarChart();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private void UpdatePolarChart()
    {
        if (_sessionService.BaseSong?.Vector == null || _sessionService.SelectedSong?.Vector == null){
            Console.WriteLine("Vector is null — chart not built");
            return;
        }
        
        Series =
        [
            new PolarLineSeries<float>
            {
                Name = _sessionService.BaseSong.SongName,
                Values = _sessionService.BaseSong.Vector,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0.2,
                IsClosed = true
            },
            new PolarLineSeries<float>
            {
                Name = _sessionService.SelectedSong.Index,
                Values = _sessionService.SelectedSong.Vector,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0.2,
                Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 2 },
                IsClosed = true,
            }
        ];
    }
    
    [RelayCommand]
    private async Task GoBack() => await Shell.Current.GoToAsync("..");

}