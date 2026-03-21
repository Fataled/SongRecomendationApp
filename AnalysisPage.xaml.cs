

using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class AnalysisPage : ContentPage
{
    public AnalysisPage(AnalysisViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}