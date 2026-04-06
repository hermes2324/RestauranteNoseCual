using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;
using Syncfusion.Maui.DataGrid;

namespace RestauranteNoseCual.View;

public partial class MisPedidosPage : ContentPage
{
    private readonly OrdenService _ordenService = new();
    private List<Pedido> _ordenes = new();

    public MisPedidosPage()
    {
        InitializeComponent();

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
    }

    private async Task CargarOrdenesAsync()
    {
        try
        {
            long clienteId = SesionService.ObtenerIdCliente();
            _ordenes = (await _ordenService.ObtenerPorClienteAsync(clienteId))
                        .OrderByDescending(o => o.FechaHora)
                        .ToList();

            if (_ordenes.Any())
            {
                GridOrdenes.ItemsSource = _ordenes;
                GridOrdenes.IsVisible = true;
                PanelVacio.IsVisible = false;
            }
            else
            {
                GridOrdenes.IsVisible = false;
                PanelVacio.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando órdenes: {ex.Message}");
        }
    }

    private async void OnOrdenTapped(object sender, DataGridCellTappedEventArgs e)
    {
        if (e.RowData is not Pedido pedido) return;

        // Quita la selección visual
        GridOrdenes.SelectedRow = null;

        await Navigation.PushAsync(new OrdenDetallePage(pedido));
    }
}