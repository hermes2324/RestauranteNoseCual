//using RestauranteNoseCual.Base_de_Datos;
//using RestauranteNoseCual.Models;
//using RestauranteNoseCual.Services;
//using Supabase.Realtime.PostgresChanges;
//using Syncfusion.Maui.DataGrid;

//namespace RestauranteNoseCual.View;

//public partial class MisPedidosPage : ContentPage
//{
//    private readonly OrdenService _ordenService = new();
//    private List<Pedido> _ordenes = new();
//    private Supabase.Realtime.RealtimeChannel _channel;

//    public MisPedidosPage()
//    {
//        InitializeComponent();
//        GridOrdenes.QueryRowHeight += (s, e) =>
//        {
//            if (e.RowIndex > 0)
//            {
//                e.Height = 52;
//                e.Handled = true;
//            }
//        };
//    }

//    protected override async void OnAppearing()
//    {
//        base.OnAppearing();
//        await CargarOrdenesAsync();
//        await IniciarRealtimeAsync();
//    }

//    protected override void OnDisappearing()
//    {
//        base.OnDisappearing();

//        if (_channel != null)
//        {
//            _channel.Unsubscribe();
//            _channel = null;
//            Console.WriteLine("[REALTIME] Canal de MisPedidos cerrado");
//        }
//    }

//    private async Task IniciarRealtimeAsync()
//    {
//        try
//        {
//            var clienteId = SesionService.ObtenerIdCliente();

//            _channel = Conexion.Supabase.Realtime.Channel(
//                $"realtime:public:Orden:ClienteId=eq.{clienteId}"
//            );

//            var postgresChanges = _channel.Register(
//                new PostgresChangesOptions("public", "Orden")
//                {
//                    Filter = $"ClienteId=eq.{clienteId}"
//                }
//            );

//            postgresChanges.AddPostgresChangeHandler(
//                PostgresChangesOptions.ListenType.Updates,
//                (_, change) =>
//                {
//                    var pedidoActualizado = change.Model<Pedido>();
//                    if (pedidoActualizado == null) return;

//                    Console.WriteLine($"[REALTIME] Orden {pedidoActualizado.Id} actualizada: {pedidoActualizado.Estado}");

//                    MainThread.BeginInvokeOnMainThread(() =>
//                    {
//                        // Buscar y actualizar el pedido en la lista existente
//                        var pedidoExistente = _ordenes.FirstOrDefault(o => o.Id == pedidoActualizado.Id);
//                        if (pedidoExistente != null)
//                        {
//                            pedidoExistente.Estado = pedidoActualizado.Estado;

//                            // Refrescar el grid
//                            GridOrdenes.ItemsSource = null;
//                            GridOrdenes.ItemsSource = _ordenes;
//                        }
//                    });
//                }
//            );

//            await _channel.Subscribe();
//            Console.WriteLine("[REALTIME] MisPedidos escuchando cambios...");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"[REALTIME] Error en MisPedidos: {ex.Message}");
//        }
//    }

//    private async Task CargarOrdenesAsync()
//    {
//        try
//        {
//            long clienteId = SesionService.ObtenerIdCliente();
//            _ordenes = (await _ordenService.ObtenerPorClienteAsync(clienteId))
//                        .OrderByDescending(o => o.FechaHora)
//                        .ToList();

//            if (_ordenes.Any())
//            {
//                GridOrdenes.ItemsSource = _ordenes;
//                GridOrdenes.IsVisible = true;
//                PanelVacio.IsVisible = false;
//            }
//            else
//            {
//                GridOrdenes.IsVisible = false;
//                PanelVacio.IsVisible = true;
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error cargando órdenes: {ex.Message}");
//        }
//    }

//    private async void OnOrdenTapped(object sender, DataGridCellTappedEventArgs e)
//    {
//        if (e.RowData is not Pedido pedido) return;
//        GridOrdenes.SelectedRow = null;
//        await Navigation.PushAsync(new OrdenDetallePage(pedido));
//    }
//} este codigo funciona pero muy lento

using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;
using Supabase.Realtime.PostgresChanges;
using Syncfusion.Maui.DataGrid;
using System.Collections.ObjectModel;

namespace RestauranteNoseCual.View;

public partial class MisPedidosPage : ContentPage
{
    private readonly OrdenService _ordenService = new();
    private ObservableCollection<Pedido> _ordenes = new();
    private Supabase.Realtime.RealtimeChannel _channel;
    private bool _cargado = false;

    public MisPedidosPage()
    {
        InitializeComponent();

        // ?? Asignar una sola vez
        GridOrdenes.ItemsSource = _ordenes;

        GridOrdenes.QueryRowHeight += (s, e) =>
        {
            if (e.RowIndex > 0)
            {
                e.Height = 52;
                e.Handled = true;
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarOrdenesAsync();
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
        // ?? No abrir canal si ya existe
        if (_channel != null) return;

        try
        {
            var clienteId = SesionService.ObtenerIdCliente();
            _channel = Conexion.Supabase.Realtime.Channel(
                $"realtime:public:Orden:ClienteId=eq.{clienteId}"
            );

            var postgresChanges = _channel.Register(
                new PostgresChangesOptions("public", "Orden")
                {
                    Filter = $"ClienteId=eq.{clienteId}"
                }
            );

            postgresChanges.AddPostgresChangeHandler(
                PostgresChangesOptions.ListenType.Updates,
                (_, change) =>
                {
                    var pedidoActualizado = change.Model<Pedido>();
                    if (pedidoActualizado == null) return;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var index = _ordenes.ToList()
                            .FindIndex(o => o.Id == pedidoActualizado.Id);

                        if (index >= 0)
                        {
                            // ?? Remove+Insert para que Syncfusion redibuje solo esa fila
                            var pedido = _ordenes[index];
                            pedido.Estado = pedidoActualizado.Estado;
                            _ordenes.RemoveAt(index);
                            _ordenes.Insert(index, pedido);
                        }
                    });
                }
            );

            await _channel.Subscribe();
            Console.WriteLine("[REALTIME] MisPedidos escuchando...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[REALTIME] Error en MisPedidos: {ex.Message}");
        }
    }

    private async Task CargarOrdenesAsync()
    {
        // ?? Solo cargar la primera vez
        if (_cargado) return;
        _cargado = true;

        try
        {
            long clienteId = SesionService.ObtenerIdCliente();
            var lista = (await _ordenService.ObtenerPorClienteAsync(clienteId))
                        .OrderByDescending(o => o.FechaHora)
                        .ToList();

            _ordenes.Clear();
            foreach (var p in lista)
                _ordenes.Add(p);

            GridOrdenes.IsVisible = _ordenes.Any();
            PanelVacio.IsVisible = !_ordenes.Any();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando órdenes: {ex.Message}");
        }
    }

    private async void OnOrdenTapped(object sender, DataGridCellTappedEventArgs e)
    {
        if (e.RowData is not Pedido pedido) return;
        GridOrdenes.SelectedRow = null;
        await Navigation.PushAsync(new OrdenDetallePage(pedido));
    }
}