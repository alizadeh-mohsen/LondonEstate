
namespace LondonEstate.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnBookingsTapped(object sender, TappedEventArgs e)
       => await Shell.Current.GoToAsync("BookingsPage");

        private async void OnGreetingsTapped(object sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync("GreetingsPage");

        private async void OnInstructionsTapped(object sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync("InstructionsPage");

        private async void OnInvoicesTapped(object sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync("InvoicePage");
    }
}