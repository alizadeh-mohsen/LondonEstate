using LondonEstate.MAUI.Services;
using System.Windows.Input;

namespace LondonEstate.MAUI.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(IAuthService authService)
    {
        InitializeComponent();
        BindingContext = new SettingsPageModel(authService);
    }
}

/// <summary>
/// View model for Settings page
/// </summary>
public class SettingsPageModel
{
    private readonly IAuthService _authService;

    public ICommand LogoutCommand { get; }

    public SettingsPageModel(IAuthService authService)
    {
        _authService = authService;
        LogoutCommand = new Command(ExecuteLogout);
    }

    /// <summary>
    /// Execute logout and navigate to login page
    /// </summary>
    private async void ExecuteLogout()
    {
        try
        {
            // Call logout (clears tokens from storage and notifies server)
            await _authService.LogoutAsync();

            // Navigate to login page
            await Shell.Current.GoToAsync("LoginPage");
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Logout Error",
                $"Failed to logout: {ex.Message}",
                "OK");
        }
    }
}
