using LondonEstate.MAUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LondonEstate.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        /// <summary>
        /// Start background services when app starts
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                var tokenRefreshManager = IPlatformApplication.Current?.Services
                    .GetService<TokenRefreshManager>();
                tokenRefreshManager?.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting token refresh manager: {ex.Message}");
            }
        }
    }
}
