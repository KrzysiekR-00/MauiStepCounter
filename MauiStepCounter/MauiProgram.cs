using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Pedometer;

namespace MauiStepCounter
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<ViewModels.MainPageViewModel>();

#if ANDROID
            builder.Services.AddSingleton(Pedometer.Default);

            builder.Services.AddTransient<Abstraction.IBackgroundService, Platforms.Android.Services.BackgroundService>();
            builder.Services.AddTransient<Abstraction.INotificationService, Platforms.Android.Services.NotificationService>();
            builder.Services.AddTransient<ActivityCore.Abstraction.IPedometer, Platforms.Android.Services.PedometerService>();
#endif

#if WINDOWS
            builder.Services.AddTransient<Abstraction.IBackgroundService, Platforms.Windows.Services.DummyBackgroundService>();
            builder.Services.AddTransient<Abstraction.INotificationService, Platforms.Windows.Services.DummyNotificationService>();
            builder.Services.AddTransient<ActivityCore.Abstraction.IPedometer, Platforms.Windows.Services.DummyPedometerService>();
#endif

            return builder.Build();
        }
    }
}
