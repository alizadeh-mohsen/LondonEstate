using CommunityToolkit.Maui;
using LondonEstate.MAUI.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace LondonEstate.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});

    				
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });
            // Register AuthService with default HttpClient (for auth endpoints)
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
#if ANDROID
                client.BaseAddress = new Uri("http://10.0.2.2:5002");
#else
                client.BaseAddress = new Uri("http://localhost:5002");
#endif
            });

            // Register authenticated HttpClient with automatic token injection
            // Use this for protected API calls
            builder.Services.AddHttpClient<AuthenticatedHttpClient>(client =>
            {
#if ANDROID
                client.BaseAddress = new Uri("http://10.0.2.2:5002");
#else
                client.BaseAddress = new Uri("http://localhost:5002");
#endif
            });

            // Register the authentication handler
            builder.Services.AddTransient<AuthenticatedHttpClientHandler>();

            // Register token refresh manager
            builder.Services.AddSingleton<TokenRefreshManager>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();

            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            return builder.Build();
        }
    }
}
