using CommunityToolkit.Maui;

using LondonEstate.MAUI.Services;
using LondonEstate.MAUI.ViewModels;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace LondonEstate.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(MauiProgram).Assembly);
                cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAxOTU4NDAwIiwiaWF0IjoiMTc3MDQ2NTk0NSIsImFjY291bnRfaWQiOiIwMTljMzdmZGUwNjc3NTQ4YTMxNTdkNjE4ODI2ZTdmZiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2d2end0MzJjOGZudDNqNzVmcjg5ZDVzIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.q4HdD44qh911D4LMxArtwGIZiPfRbi_eMuMFrrzA7egheI2uZDUo3WPJlkKMGrpVaoZqREHyOY3j0sCq3wq50E7SKD9FA7F33eIUaD5AhKBvoB4yOu75hPDHrfceRpes8luDlTqYjrIZy91A2Gyjou8IkJrzPsrH6NCrv1vgtklRnkWA2qaE5hUkx6ML7uFpe2l4swCikBG66BIe5xuwvOc5fU6HekJNkJw3er_mi4ZdjWP7ey42q7Sc9o531wWZBs6B-bnXIsf_FxCd3v3UBClNVHDYv9F8HApF-OurUe7RAnChq1Tv3U3tsIMQHWHZo3AsMcIW5A61HVZiVTnZnQ";
            });
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

            // Register the authentication handler
            builder.Services.AddTransient<AuthenticatedHttpClientHandler>();

            // Register token refresh manager
            builder.Services.AddSingleton<TokenRefreshManager>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddScoped<IFlatService, FlatService>();
            builder.Services.AddSingleton<BookingsViewModel>();
            builder.Services.AddTransient<BookingsPage>();
            builder.Services.AddTransient<GreetingsPage>();
            builder.Services.AddTransient<InstructionPage>();
            builder.Services.AddTransient<InvoicePage>();


#if DEBUG
            builder.Logging.AddDebug();

            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            return builder.Build();
        }
    }
}
