using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class AccountCreationPage : ContentPage
{
    public AccountCreationPage(RegisterPageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}