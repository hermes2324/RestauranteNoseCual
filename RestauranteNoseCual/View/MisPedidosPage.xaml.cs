using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class MisPedidosPage : ContentPage
{
    private readonly OrdenService _ordenService = new();

    public MisPedidosPage()
    {
        InitializeComponent();
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
            // Obtiene el id del cliente desde la sesión
            long clienteId = SesionService.ObtenerIdCliente();
            var ordenes = await _ordenService.ObtenerPorClienteAsync(clienteId);
            ListaOrdenes.ItemsSource = ordenes.OrderByDescending(o => o.FechaHora).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando órdenes: {ex.Message}");
        }
    }

    private async void OnOrdenSeleccionada(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Pedido pedido)
            return;

        ListaOrdenes.SelectedItem = null;

        // Reutiliza tu OrdenDetallePage existente
        await Navigation.PushAsync(new OrdenDetallePage(pedido));
    }
}