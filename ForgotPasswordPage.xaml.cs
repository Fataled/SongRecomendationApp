using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(RegisterPageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}