using RestauranteNoseCual.Models;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Services;
using Syncfusion.Maui.DataGrid;

namespace RestauranteNoseCual.View
{
    public partial class CarritoPage : ContentPage
    {
        //private readonly Mesa _mesa;
        private readonly OrdenService _ordenService = new();
        private readonly CarritoController _carritoController = new();

        private readonly Mesa? _mesa;

        public CarritoPage(Mesa? mesa)
        {
            InitializeComponent();
            _mesa = mesa;

            if (_mesa == null) 
            {
                PkrEntrega.SelectedItem = "Domicilio";
                PkrEntrega.IsEnabled = false;

               
                EntNombreCliente.Text = PedidoTemporal.NombreCliente;
            }
            else
            {
                PkrEntrega.SelectedItem = "Mesa";
                PedidoTemporal.NombreCliente = string.Empty;
            }

            ActualizarTotal();
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
            // 1. Validaciones iniciales
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

            // 2. Determinar destino y tipo de entrega
            bool esDomicilio = _mesa == null;
            string destino = !esDomicilio ? $"Mesa {_mesa.Numero}" : "Domicilio";
            string tipoEntrega = PkrEntrega.SelectedItem?.ToString() ?? (esDomicilio ? "Domicilio" : "Mesa");

            // 3. Mensaje de confirmación
            bool confirmar = await DisplayAlert("Confirmar",
                $"¿{destino} - Cliente: {EntNombreCliente.Text}?", "Sí", "No");

            if (!confirmar) return;

            OverlayCarga.IsVisible = true;
            Loader.IsRunning = true;

            try
            {
                // 4. Llamada al servicio pasando TODOS los parámetros del PedidoTemporal
                bool exito = await _ordenService.GuardarOrdenAsync(
                    items: CarritoController.Items.ToList(),
                    mesaId: _mesa?.Id,
                    nombreCliente: EntNombreCliente.Text.Trim(),
                    tipoEntrega: tipoEntrega,
                    clienteId: PedidoTemporal.IdCliente,     
                    notas: PedidoTemporal.Notas,             
                    costoEnvio: esDomicilio ? 20m : 0m        
                );

                if (exito)
                {
                    // 5. Limpiar datos tras el éxito
                    LimpiarDatosTemporales();
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

        // Método auxiliar para resetear la clase estática y que no se mezclen pedidos
        private void LimpiarDatosTemporales()
        {
            PedidoTemporal.IdCliente = null;
            PedidoTemporal.NombreCliente = "";
            PedidoTemporal.Notas = "";
            PedidoTemporal.Direccion = "";
        }


    }
}