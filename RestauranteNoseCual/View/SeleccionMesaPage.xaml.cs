using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;

namespace RestauranteNoseCual.View;

public partial class SeleccionMesaPage : ContentPage
{
    private readonly MesaController _mesaController = new();

    public SeleccionMesaPage()
    {
        InitializeComponent();
        CargarMesasAsync();
    }

    private async void CargarMesasAsync()
    {
        var mesas = await _mesaController.ObtenerMesasAsync();

        ListaMesas.ItemsSource = mesas;
        Cargando.IsVisible = false;
        Cargando.IsRunning = false;
        ListaMesas.IsVisible = true;
    }

    private async void OnMesaSeleccionada(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Mesa mesa) return;

      
        if (mesa.Estado == "Ocupada")
        {
            await DisplayAlert("Mesa ocupada",
                               $"La Mesa {mesa.Numero} ya está ocupada",
                               "OK");
            return;
        }

    
        bool confirmar = await DisplayAlert(
            "Confirmar",
            $"¿Iniciar orden para la Mesa {mesa.Numero}?",
            "Sí", "No");

        if (!confirmar) return;

        
        await _mesaController.OcuparMesaAsync(mesa.Id);

        
        await Navigation.PushAsync(new MenuPage(mesa));
    }

    private async void OnPedidoDomicilioClicked(object sender, EventArgs e)
    {
    
        await Navigation.PushAsync(new PedidoDomicilioPage());
    }
}