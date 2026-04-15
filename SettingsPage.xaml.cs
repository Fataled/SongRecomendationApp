using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

// Authored by Brume
public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}