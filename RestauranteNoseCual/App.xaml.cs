using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
using Plugin.LocalNotification;
using RestauranteNoseCual.Services;
using RestauranteNoseCual.Base_de_Datos;    

namespace RestauranteNoseCual
{
    public partial class App : Application
    {
        private readonly RealtimeNotificationService _realtimeService = new();
        //este si jala
        //public App()
        //{
        //    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCe0x3WmFZfVhgdl9CYFZSQmYuP1ZhSXxVdkFjWH9cdX1UQWlbWEB9XEE=");
        //    InitializeComponent();

        //    if (SesionService.HaySesionActiva())
        //    {
        //        MainPage = new View.FlyoutMenuPage();

        //        // 👇 Si ya hay sesión activa iniciar escucha inmediatamente
        //        Task.Run(async () => await _realtimeService.IniciarEscuchaAsync());
        //    }
        //    else
        //    {
        //        MainPage = new NavigationPage(new View.Inicio_Sesion());
        //    }

        //    CrossFirebaseCloudMessaging.Current.NotificationReceived += OnNotificacionRecibida;
        //}
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCe0x3WmFZfVhgdl9CYFZSQmYuP1ZhSXxVdkFjWH9cdX1UQWlbWEB9XEE=");
            InitializeComponent();

            if (SesionService.HaySesionActiva())
            {
                MainPage = new View.FlyoutMenuPage();
            }
            else
            {
                MainPage = new NavigationPage(new View.Inicio_Sesion());
            }

            CrossFirebaseCloudMessaging.Current.NotificationReceived += OnNotificacionRecibida;

            // 👇 Iniciar realtime siempre al arrancar
            Task.Run(async () =>
            {
                try
                {
                    await Conexion.Supabase.InitializeAsync();
                    Console.WriteLine("[REALTIME] Supabase inicializado");
                    var realtimeService = new RestauranteNoseCual.Services.RealtimeNotificationService();
                    await realtimeService.IniciarEscuchaAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[REALTIME] Error al iniciar: {ex.Message}");
                }
            });
        }

        // 👇 Método público para llamar después del login
        public void IniciarRealtimeNotifications()
        {
            Task.Run(async () => await _realtimeService.IniciarEscuchaAsync());
        }

        private void OnNotificacionRecibida(object sender, FCMNotificationReceivedEventArgs e)
        {
            Console.WriteLine($"[FCM] Evento disparado: {e.Notification?.Title}");
            if (SesionService.ObtenerRol() != "Mesero") return;

            var titulo = e.Notification?.Title ?? "Nueva orden";
            var cuerpo = e.Notification?.Body ?? "Tienes una nueva orden";

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var notification = new NotificationRequest
                {
                    NotificationId = new Random().Next(1000, 9999),
                    Title = titulo,
                    Description = cuerpo,
                };
                LocalNotificationCenter.Current.Show(notification);
            });
        }
    }
}