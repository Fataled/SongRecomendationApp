using System.Diagnostics;
using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Models;
using ProjectHellsParadise.BusinessLogic.ViewModels;

namespace ProjectHellsParadise;

public partial class MainPage : ContentPage
{
    
    public MainPage(RegisterPageViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}