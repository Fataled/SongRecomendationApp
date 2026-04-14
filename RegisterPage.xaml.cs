using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

// Authored by Brume
public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterPageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}