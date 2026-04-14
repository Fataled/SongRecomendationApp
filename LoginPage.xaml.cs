using System.Diagnostics;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;
//Authored by Brume
public partial class LoginPage : ContentPage
{
    
    public LoginPage(RegisterPageViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}