using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class LoginPage : ContentPage
{
    public LoginPage(RegisterPageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}