using LondonEstate.MAUI.Models;
using LondonEstate.MAUI.PageModels;

namespace LondonEstate.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}