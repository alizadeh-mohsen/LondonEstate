
namespace LondonEstate.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnBookingsTapped(object sender, TappedEventArgs e)
           => await Shell.Current.GoToAsync(nameof(BookingsPage));

        private async void OnGreetingsTapped(object sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync(nameof(GreetingsPage));

        private async void OnInstructionsTapped(object sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync(nameof(InstructionPage));

        private async void OnInvoicesTapped(object sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync(nameof(InvoicePage));
    }
}