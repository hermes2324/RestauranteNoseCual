using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class PedidoDomicilioPage : ContentPage
{
    private readonly ClienteService _clienteService = new();

    public PedidoDomicilioPage()
    {
        InitializeComponent();
    }

    // Busca si el cliente ya existe en Supabase por su teléfono
    private async void OnBuscarClienteClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntBuscarTel.Text)) return;

        var cliente = await _clienteService.BuscarPorTelefonoAsync(EntBuscarTel.Text.Trim());
        if (cliente != null)
        {
            PedidoTemporal.IdCliente = cliente.Id;
            EntNombre.Text = cliente.Nombre;
            EntTelefono.Text = cliente.Telefono;
            EntDomicilio.Text = cliente.Domicilio;
            EntNotas.Text = cliente.UltimasNotas;
            PedidoTemporal.Notas = cliente.UltimasNotas;
            await DisplayAlert("Cliente Encontrado", $"Bienvenido de nuevo {cliente.Nombre}", "OK");
        }
        else
        {
            await DisplayAlert("Nuevo Cliente", "No se encontraron datos, favor de registrar.", "OK");
            EntTelefono.Text = EntBuscarTel.Text; 
        }

        PanelCliente.IsVisible = true;
    }

    private async void OnContinuarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntNombre.Text))
        {
            await DisplayAlert("Error", "El nombre es obligatorio", "OK");
            return;
        }

        
        PedidoTemporal.NombreCliente = EntNombre.Text;
        PedidoTemporal.Telefono = EntTelefono.Text;
        PedidoTemporal.Direccion = EntDomicilio.Text;
        PedidoTemporal.Notas = EntNotas.Text;
        PedidoTemporal.CostoEnvio = 20; 

        await Navigation.PushAsync(new MenuPage(null));
    }
}