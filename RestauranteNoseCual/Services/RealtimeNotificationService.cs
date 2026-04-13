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

            // 👇 Escuchar nuevas órdenes (INSERT) - para meseros
            var postgresChanges = channel.Register(new PostgresChangesOptions("public", "Orden"));

            postgresChanges.AddPostgresChangeHandler(
            PostgresChangesOptions.ListenType.Inserts,
            (_, change) =>
            {
                var pedido = change.Model<Pedido>();
                if (pedido == null) return;
       
                // 👇 No notificar si el pedido lo hizo el mismo mesero
                var idActual = SesionService.ObtenerIdCliente();
                if (pedido.ClienteId == idActual)
                {
                    Console.WriteLine("[REALTIME] Pedido propio, ignorando notificación");
                    return;
                }
       
                Console.WriteLine("[REALTIME] Nueva orden detectada");
                var titulo = "🍽️ Nueva orden";
                var cuerpo = $"{pedido.NombreCliente} - {pedido.TipoEntrega}";
                MostrarNotificacionLocal(titulo, cuerpo);
            }
        );

            await channel.Subscribe();
            Console.WriteLine("[REALTIME] Escuchando nuevas órdenes...");
        }

        // 👇 Nuevo método para clientes
        public async Task IniciarEscuchaClienteAsync(long clienteId)
        {
            if (SesionService.ObtenerRol() != "Cliente") return;

            Console.WriteLine("[REALTIME] Cliente escuchando cambios de orden...");

            // 👇 Guardar estados actuales antes de suscribirse
            var estadosConocidos = new Dictionary<long, string>();

            var ordenesActuales = await _supabase
                .From<Pedido>()
                .Where(p => p.ClienteId == clienteId)
                .Get();

            foreach (var orden in ordenesActuales.Models)
            {
                estadosConocidos[orden.Id] = orden.Estado;
                Console.WriteLine($"[REALTIME] Estado conocido orden {orden.Id}: {orden.Estado}");
            }

            var channel = _supabase.Realtime.Channel($"realtime:public:Orden:ClienteId=eq.{clienteId}");

            var postgresChanges = channel.Register(
                new PostgresChangesOptions("public", "Orden")
                {
                    Filter = $"ClienteId=eq.{clienteId}"
                }
            );

            postgresChanges.AddPostgresChangeHandler(
                PostgresChangesOptions.ListenType.Updates,
                (_, change) =>
                {
                    var pedido = change.Model<Pedido>();
                    if (pedido?.Estado == null) return;

                   
                    if (estadosConocidos.TryGetValue(pedido.Id, out var estadoAnterior))
                    {
                        if (estadoAnterior == pedido.Estado)
                        {
                            Console.WriteLine($"[REALTIME] Estado sin cambios, ignorando");
                            return;
                        }
                    }

                    // Actualizar estado conocido
                    estadosConocidos[pedido.Id] = pedido.Estado;

                    Console.WriteLine($"[REALTIME] Estado cambió a: {pedido.Estado}");
                    var titulo = "📋 Tu pedido fue actualizado";
                    var cuerpo = $"El estado de tu pedido es: {pedido.Estado}";
                    MostrarNotificacionLocal(titulo, cuerpo);
                }
            );

            await channel.Subscribe();
            Console.WriteLine("[REALTIME] Cliente escuchando...");
        }

        private void MostrarNotificacionLocal(string titulo, string cuerpo)
        {
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