using LondonEstate.MAUI.Services.Api;

namespace LondonEstate.MAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthApi _authApi;

    public LoginPage()
    {
        InitializeComponent();
        _authApi = new AuthApi();
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
            var token = await _authApi.LoginAsync(email, password);

            await SecureStorage.SetAsync("auth_token", token);

            // Build authenticated shell
            (Application.Current.MainPage as AppShell)?.BuildAuthenticatedShell();

            // Navigate to dashboard
            await Shell.Current.GoToAsync("//main");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Invalid login. Please try again.";
            ErrorLabel.IsVisible = true;
        }
    }
}
