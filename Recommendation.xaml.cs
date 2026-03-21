using ProjectHellsParadise.BusinessLogic.ViewModels;


namespace ProjectHellsParadise;

public partial class Recommendation : ContentPage
{
    public Recommendation(RecommendationViewModel vm)
    { 
        BindingContext = vm;
       InitializeComponent();
    }
}