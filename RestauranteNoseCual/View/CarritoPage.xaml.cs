using RestauranteNoseCual.Models;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Services;
using Syncfusion.Maui.DataGrid;

namespace RestauranteNoseCual.View
{
    public partial class CarritoPage : ContentPage
    {
        private readonly Mesa _mesa;
        private readonly OrdenService _ordenService = new();
        private readonly CarritoController _carritoController = new();

        public CarritoPage(Mesa mesa)
        {
            InitializeComponent();
            _mesa = mesa;

           
            LblFecha.Text = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ActualizarTotal();

            if (PkrEntrega.Items.Count > 0)
                PkrEntrega.SelectedIndex = 0;
        }

        private void ActualizarTotal()
        {
            if (LblTotal != null)
                LblTotal.Text = _carritoController.CalcularTotal().ToString("C2");
        }

        private void OnEliminarItem(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is CarritoItem item)
            {
                _carritoController.EliminarItem(item);
                ActualizarTotal();
            }
        }

        private async void OnConfirmarPedido(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntNombreCliente.Text))
            {
                await DisplayAlert("Atención", "Escribe el nombre del cliente.", "OK");
                return;
            }

            if (CarritoController.Items == null || CarritoController.Items.Count == 0)
            {
                await DisplayAlert("Vacío", "No hay productos en el carrito.", "OK");
                return;
            }

            bool confirmar = await DisplayAlert("Confirmar",
                $"¿Mesa {_mesa.Numero} - Cliente: {EntNombreCliente.Text}?", "Sí", "No");

            if (!confirmar) return;

            OverlayCarga.IsVisible = true;
            Loader.IsRunning = true;

            try
            {
                bool exito = await _ordenService.GuardarOrdenAsync(CarritoController.Items.ToList(), _mesa.Id);

                if (exito)
                {
                    CarritoController.Items.Clear();
                    await DisplayAlert("¡Éxito!", "Orden enviada a cocina.", "OK");
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo guardar. Revisa internet.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error Crítico", ex.Message, "OK");
            }
            finally
            {
                OverlayCarga.IsVisible = false;
                Loader.IsRunning = false;
            }
        }
    }
}