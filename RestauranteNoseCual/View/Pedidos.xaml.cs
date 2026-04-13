//using Microsoft.Maui.Controls;
//using RestauranteNoseCual.Base_de_Datos;
//using RestauranteNoseCual.Controllers;
//using RestauranteNoseCual.Models;
//using Supabase.Realtime.PostgresChanges;

//namespace RestauranteNoseCual.View
//{
//    public partial class Pedidos : ContentPage
//    {
//        private readonly PedidosController _controller = new();
//        private Supabase.Realtime.RealtimeChannel _channel;

//        public Pedidos()
//        {
//            InitializeComponent();
//            GridPedidos.ItemsSource = _controller.ListaPedidos;
//            FiltroPicker.SelectedIndex = 0;
//        }

//        protected override async void OnAppearing()
//        {
//            base.OnAppearing();
//            await _controller.CargarPedidosAsync();
//            RefrescarGrid();
//            await IniciarRealtimeAsync();
//        }

//        protected override void OnDisappearing()
//        {
//            base.OnDisappearing();
//            if (_channel != null)
//            {
//                _channel.Unsubscribe();
//                _channel = null;
//                Console.WriteLine("[REALTIME] Canal de Pedidos cerrado");
//            }
//        }

//        private async Task IniciarRealtimeAsync()
//        {
//            try
//            {
//                _channel = Conexion.Supabase.Realtime.Channel("realtime:public:Orden");

//                var postgresChanges = _channel.Register(
//                    new PostgresChangesOptions("public", "Orden")
//                );

//                postgresChanges.AddPostgresChangeHandler(
//                    PostgresChangesOptions.ListenType.Updates,
//                    (_, change) =>
//                    {
//                        var pedidoActualizado = change.Model<Pedido>();
//                        if (pedidoActualizado == null) return;

//                        Console.WriteLine($"[REALTIME] Pedido {pedidoActualizado.Id} actualizado: {pedidoActualizado.Estado}");

//                        MainThread.BeginInvokeOnMainThread(() =>
//                        {
//                            // Buscar y actualizar en la lista del controller
//                            var pedidoExistente = _controller.ListaPedidos
//                                .FirstOrDefault(p => p.Id == pedidoActualizado.Id);

//                            if (pedidoExistente != null)
//                            {
//                                pedidoExistente.Estado = pedidoActualizado.Estado;
//                                RefrescarGrid();
//                            }
//                        });
//                    }
//                );

//                await _channel.Subscribe();
//                Console.WriteLine("[REALTIME] Pedidos del mesero escuchando cambios...");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[REALTIME] Error en Pedidos: {ex.Message}");
//            }
//        }

//        private async void OnFiltroChanged(object sender, EventArgs e)
//        {
//            string filtro = FiltroPicker.SelectedItem?.ToString() ?? "Todos";
//            var filtrados = await _controller.FiltrarPorTipoAsync(filtro);
//            GridPedidos.ItemsSource = null;
//            GridPedidos.ItemsSource = filtrados;
//        }

//        private async void OnVerDetalleClicked(object sender, EventArgs e)
//        {
//            if (sender is Button btn && btn.CommandParameter is Pedido pedido)
//                await Navigation.PushAsync(new OrdenDetallePage(pedido));
//        }

//        private void RefrescarGrid()
//        {
//            GridPedidos.ItemsSource = null;
//            GridPedidos.ItemsSource = _controller.ListaPedidos;
//        }

//        public List<string> ListaEstados { get; set; } = new()
//        {
//            "En preparación",
//            "En proceso de entrega",
//            "Entregado"
//        };

//        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            var picker = sender as Picker;
//            if (picker?.BindingContext is Pedido pedido)
//            {
//                string nuevoEstado = pedido.Estado;
//                await _controller.ActualizarEstadoAsync(pedido, nuevoEstado);
//            }
//        }
//    }
//}
//esta version funciona muyyy lenta
using Microsoft.Maui.Controls;
using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;
using Supabase.Realtime.PostgresChanges;

namespace RestauranteNoseCual.View
{
    public partial class Pedidos : ContentPage
    {
        private readonly PedidosController _controller = new();
        private Supabase.Realtime.RealtimeChannel _channel;

        public Pedidos()
        {
            InitializeComponent();
            // 👇 Asignar una sola vez, nunca volver a tocar
            GridPedidos.ItemsSource = _controller.ListaPedidos;
            FiltroPicker.SelectedIndex = 0;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _controller.CargarPedidosAsync();
            await IniciarRealtimeAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_channel != null)
            {
                _channel.Unsubscribe();
                _channel = null;
            }
        }

        private async Task IniciarRealtimeAsync()
        {
            if (_channel != null) return;

            try
            {
                _channel = Conexion.Supabase.Realtime.Channel("realtime:public:Orden");
                var postgresChanges = _channel.Register(new PostgresChangesOptions("public", "Orden"));

                // 👇 Escuchar nuevos pedidos (INSERT)
                postgresChanges.AddPostgresChangeHandler(
                    PostgresChangesOptions.ListenType.Inserts,
                    (_, change) =>
                    {
                        var nuevoPedido = change.Model<Pedido>();
                        if (nuevoPedido == null) return;

                        Console.WriteLine($"[REALTIME] Nuevo pedido: {nuevoPedido.Id}");

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // 👇 Insertar al inicio para que aparezca primero
                            _controller.ListaPedidos.Insert(0, nuevoPedido);
                        });
                    }
                );

                // 👇 Escuchar cambios de estado (UPDATE)
                postgresChanges.AddPostgresChangeHandler(
                    PostgresChangesOptions.ListenType.Updates,
                    (_, change) =>
                    {
                        var pedidoActualizado = change.Model<Pedido>();
                        if (pedidoActualizado == null) return;

                        Console.WriteLine($"[REALTIME] Pedido {pedidoActualizado.Id} → {pedidoActualizado.Estado}");

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            var index = -1;
                            for (int i = 0; i < _controller.ListaPedidos.Count; i++)
                            {
                                if (_controller.ListaPedidos[i].Id == pedidoActualizado.Id)
                                {
                                    index = i;
                                    break;
                                }
                            }

                            if (index >= 0)
                            {
                                // 👇 Quitar y reinsertar fuerza a Syncfusion a redibujar solo esa fila
                                var pedido = _controller.ListaPedidos[index];
                                pedido.Estado = pedidoActualizado.Estado;
                                _controller.ListaPedidos.RemoveAt(index);
                                _controller.ListaPedidos.Insert(index, pedido);
                            }
                        });
                    }
                );

                await _channel.Subscribe();
                Console.WriteLine("[REALTIME] Pedidos escuchando...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REALTIME] Error: {ex.Message}");
            }
        }

        private async void OnFiltroChanged(object sender, EventArgs e)
        {
            string filtro = FiltroPicker.SelectedItem?.ToString() ?? "Todos";
            if (filtro == "Todos")
            {
                GridPedidos.ItemsSource = _controller.ListaPedidos;
            }
            else
            {
                var filtrados = await _controller.FiltrarPorTipoAsync(filtro);
                GridPedidos.ItemsSource = filtrados;
            }
        }

        private async void OnVerDetalleClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Pedido pedido)
                await Navigation.PushAsync(new OrdenDetallePage(pedido));
        }

        public List<string> ListaEstados { get; set; } = new()
        {
            "En preparación",
            "En proceso de entrega",
            "Entregado"
        };

        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker?.BindingContext is Pedido pedido)
            {
                string nuevoEstado = pedido.Estado;
                await _controller.ActualizarEstadoAsync(pedido, nuevoEstado);
            }
        }
    }
}