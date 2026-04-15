using ProjectHellsParadise.BusinessLogic.ViewModels;


namespace ProjectHellsParadise;
// Authored by Brume
public partial class Recommendation : ContentPage
{
    public Recommendation(RecommendationViewModel vm)
    { 
        BindingContext = vm;
       InitializeComponent();
    }
}