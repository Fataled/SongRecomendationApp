using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

// Authored by Brume
public partial class SongSearchPage : ContentPage
{

    public SongSearchPage(SongSearchViewModel viewModel)
    {
       
        BindingContext = viewModel;
        InitializeComponent();
        
    }
    
}