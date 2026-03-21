using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class SongSearchPage : ContentPage
{

    public SongSearchPage(SongSearchViewModel viewModel)
    {
       
        BindingContext = viewModel;
        InitializeComponent();
        
    }
    
}