using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;
// Authored by Brume
public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}