using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using RestauranteNoseCual.Base_de_Datos;
using Syncfusion.Maui.Core.Hosting;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Core;

namespace RestauranteNoseCual
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterFirebaseServices(); // 👈 agrega esto

            builder.UseLocalNotification();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}