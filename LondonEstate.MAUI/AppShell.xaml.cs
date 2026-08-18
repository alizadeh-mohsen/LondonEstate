using LondonEstate.MAUI.Services;

namespace LondonEstate.MAUI
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell()
        {
            InitializeComponent();

            // Get auth service from DI container
            _authService = IPlatformApplication.Current?.Services.GetService<IAuthService>()!;

            var currentTheme = Application.Current!.RequestedTheme;

            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("main", typeof(MainPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            //Routing.RegisterRoute("projects", typeof(ProjectListPage));
            //Routing.RegisterRoute("manage", typeof(ManageMetaPage));
        }

        /// <summary>
        /// Called when shell appears - checks if user is still authenticated
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckAuthenticationStateAsync();
        }

        /// <summary>
        /// Checks if user is authenticated and restores shell state
        /// </summary>
        private async Task CheckAuthenticationStateAsync()
        {
            try
            {
                // Check if user has valid token
                bool isAuthenticated = await _authService.IsAuthenticatedAsync();

                if (isAuthenticated)
                {
                    // User is logged in, build authenticated shell
                    BuildAuthenticatedShell();
                }
                else
                {
                    // Check if refresh token exists for automatic re-authentication
                    var refreshToken = await SecureStorage.GetAsync("refresh_token");

                    if (!string.IsNullOrWhiteSpace(refreshToken))
                    {
                        try
                        {
                            // Try to refresh token
                            await _authService.RefreshTokenAsync(refreshToken);
                            BuildAuthenticatedShell();
                        }
                        catch
                        {
                            // Refresh failed, show login
                            ShowLoginPage();
                        }
                    }
                    else
                    {
                        // No token or refresh token, show login
                        ShowLoginPage();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Authentication check error: {ex.Message}");
                ShowLoginPage();
            }
        }

        /// <summary>
        /// Navigate to login page
        /// </summary>
        private async Task ShowLoginPage()
        {
            await Shell.Current.GoToAsync("LoginPage");
        }

        /// <summary>
        /// Build the authenticated shell with menu items
        /// </summary>
        public void BuildAuthenticatedShell()
        {
            // Clear login-only shell
            Items.Clear();

            // Add authenticated pages
            Items.Add(new FlyoutItem
            {
                Title = "Dashboard",
                Icon = "dashboard.png",
                Items =
            {
                new ShellContent
                {
                    Title = "Dashboard",
                    ContentTemplate = new DataTemplate(typeof(MainPage)),
                    Route = "main"
                }
            }
            });

            Items.Add(new FlyoutItem
            {
                Title = "Projects",
                Icon = "projects.png",
                Items =
            {
                new ShellContent
                {
                    Title = "Projects",
                    //ContentTemplate = new DataTemplate(typeof(ProjectListPage)),
                    Route = "projects"
                }
            }
            });

            Items.Add(new FlyoutItem
            {
                Title = "Manage Meta",
                Icon = "meta.png",
                Items =
            {
                new ShellContent
                {
                    Title = "Manage Meta",
                    //ContentTemplate = new DataTemplate(typeof(ManageMetaPage)),
                    Route = "manage"
                }
            }
            });

            // Re-enable flyout
            FlyoutBehavior = FlyoutBehavior.Flyout;
        }
    }
}
