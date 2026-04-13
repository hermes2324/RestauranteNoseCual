using Plugin.LocalNotification;
using RestauranteNoseCual.Base_de_Datos;
using Supabase.Realtime.PostgresChanges;

namespace RestauranteNoseCual.Services
{
    public class RealtimeNotificationService
    {
        private readonly Supabase.Client _supabase = Conexion.Supabase;

        public async Task IniciarEscuchaAsync()
        {
            if (SesionService.ObtenerRol() != "Mesero") return;

            Console.WriteLine("[REALTIME] Iniciando escucha...");

            var channel = _supabase.Realtime.Channel("realtime:public:Orden");

            var postgresChanges = channel.Register(new PostgresChangesOptions("public", "Orden"));

            postgresChanges.AddPostgresChangeHandler(
                PostgresChangesOptions.ListenType.Inserts,
                (_, change) =>
                {
                    Console.WriteLine("[REALTIME] Nueva orden detectada");

                    var pedido = change.Model<Pedido>();
                    var titulo = "🍽️ Nueva orden";
                    var cuerpo = $"{pedido?.NombreCliente} - {pedido?.TipoEntrega}";

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
            );

            await channel.Subscribe();
            Console.WriteLine("[REALTIME] Escuchando nuevas órdenes...");
        }
    }
}