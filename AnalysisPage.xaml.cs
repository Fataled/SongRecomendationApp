using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class AnalysisPage : ContentPage
{
    public AnalysisPage(AnalysisViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}