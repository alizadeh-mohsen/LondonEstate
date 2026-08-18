using LondonEstate.MAUI.Services;

namespace LondonEstate.MAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _authService;

    public LoginPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Please enter both email and password.";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            // Login (token is automatically stored by AuthService)
            await _authService.LoginAsync(email, password);

            // Build authenticated shell
            (Application.Current.MainPage as AppShell)?.BuildAuthenticatedShell();

            // Navigate to dashboard
            await Shell.Current.GoToAsync("//main");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message.Contains("401") 
                ? "Invalid email or password." 
                : "Login failed. Please try again.";
            ErrorLabel.IsVisible = true;

            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
        }
    }
}
