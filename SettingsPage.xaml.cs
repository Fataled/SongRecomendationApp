using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageViewModel  viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}