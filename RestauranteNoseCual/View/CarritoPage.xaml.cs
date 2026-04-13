//using RestauranteNoseCual.Models;
//using RestauranteNoseCual.Controllers;
//using RestauranteNoseCual.Services;
//using Plugin.LocalNotification;

//namespace RestauranteNoseCual.View
//{
//    public partial class CarritoPage : ContentPage
//    {
//        private readonly OrdenService _ordenService = new();
//        private readonly CarritoController _carritoController = new();
//        private readonly ClienteService _clienteService = new();
//        private readonly Mesa? _mesa;
//        SeleccionMesaPage seleccionMesaPage = new SeleccionMesaPage();
//        Cliente clienteDom = new Cliente();

//        public CarritoPage(Mesa? mesa)
//        {
//            InitializeComponent();
//            _mesa = mesa;

//            if (SesionService.HaySesionActiva())
//            {
//                var (correo, nombre) = SesionService.ObtenerSesion();
//                string rol = SesionService.ObtenerRol();

//                if (rol == "Cliente")
//                {
//                    EntNombreCliente.Text = nombre;
//                    PedidoTemporal.NombreCliente = nombre;
//                    PedidoTemporal.IdCliente = Preferences.Get("sesion_id", (long)0);
//                }
//            }

//            if (_mesa == null)
//            {
//                PkrEntrega.SelectedItem = "Domicilio";
//                PkrEntrega.IsEnabled = false;
//                if (string.IsNullOrEmpty(EntNombreCliente.Text))
//                    EntNombreCliente.Text = PedidoTemporal.NombreCliente;
//            }
//            else
//            {
//                PkrEntrega.SelectedItem = "Mesa";
//            }

//            ActualizarTotal();
//        }

//        //public CarritoPage(Cliente cliente)
//        //{
//        //    clienteDom = cliente;
//        //    EntNombreCliente.Text = cliente.Nombre;
//        //    EntDomicilioCliente.Text = cliente.Domicilio;
//        //}
//        protected override async void OnAppearing()
//        {
//            base.OnAppearing();

//            string rol = SesionService.ObtenerRol();

//            if (rol == "Cliente" && _mesa == null)
//            {
//                PanelDomicilio.IsVisible = true;
//                await CargarDatosClienteAsync();
//            }

//            RefrescarLista();
//            ActualizarTotal();
//        }

//        private async Task CargarDatosClienteAsync()
//        {
//            try
//            {
//                var (correo, _) = SesionService.ObtenerSesion();
//                var cliente = await _clienteService.ObtenerPorCorreoAsync(correo);

//                if (cliente != null)
//                {
//                    EntDomicilioCliente.Text = cliente.Domicilio;
//                    EntNotasCliente.Text = cliente.Notas;
//                    PedidoTemporal.Notas = cliente.Notas;
//                    PedidoTemporal.Direccion = cliente.Domicilio;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error cargando datos cliente: {ex.Message}");
//            }
//        }

//        private void ActualizarTotal()
//        {
//            if (LblTotal == null) return;

//            bool esDomicilio = _mesa == null && SesionService.ObtenerRol() == "Cliente";
//            decimal subtotal = _carritoController.CalcularTotal();
//            decimal envio = esDomicilio ? 20m : 0m;
//            decimal total = subtotal + envio;

//            LblTotal.Text = total.ToString("C2");

//            if (FilaEnvio != null)
//                FilaEnvio.IsVisible = esDomicilio;
//        }

//        private void RefrescarLista()
//        {
//            ListaCarrito.ItemsSource = null;
//            ListaCarrito.ItemsSource = CarritoController.Items;
//        }

//        private void OnSumarCantidad(object sender, TappedEventArgs e)
//        {
//            if (e.Parameter is not CarritoItem item) return;

//            item.Cantidad++;


//            RefrescarLista();
//            ActualizarTotal();
//        }

//        private async void OnRestarCantidad(object sender, TappedEventArgs e)
//        {
//            if (e.Parameter is not CarritoItem item) return;

//            if (item.Cantidad <= 1)
//            {
//                bool confirmar = await DisplayAlert(
//                    "Eliminar producto",
//                    $"¿Quitar '{item.Producto.Nombre}' del carrito?",
//                    "Sí", "No");

//                if (!confirmar) return;

//                _carritoController.EliminarItem(item);
//            }
//            else
//            {
//                item.Cantidad--;

//            }

//            RefrescarLista();
//            ActualizarTotal();
//        }

//        private async void OnConfirmarPedido(object sender, EventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(EntNombreCliente.Text))
//            {
//                await DisplayAlert("Atención", "Escribe el nombre del cliente.", "OK");
//                return;
//            }

//            if (CarritoController.Items == null || CarritoController.Items.Count == 0)
//            {
//                await DisplayAlert("Vacío", "No hay productos en el carrito.", "OK");
//                return;
//            }

//            bool esDomicilio = _mesa == null;
//            string rol = SesionService.ObtenerRol();
//            bool esClienteDomicilio = esDomicilio && rol == "Cliente";

//            if (esClienteDomicilio)
//            {
//                var (correo, _) = SesionService.ObtenerSesion();
//                var cliente = await _clienteService.ObtenerPorCorreoAsync(correo);

//                bool perfilIncompleto = cliente == null
//                    || string.IsNullOrWhiteSpace(cliente.Telefono)
//                    || string.IsNullOrWhiteSpace(cliente.Domicilio);

//                if (perfilIncompleto)
//                {
//                    bool irAPerfil = await DisplayAlert(
//                        "Perfil incompleto",
//                        "Necesitas teléfono y domicilio para hacer un pedido a domicilio.\n¿Ir a tu perfil ahora?",
//                        "Sí, completar", "Cancelar");

//                    if (irAPerfil)
//                        await Navigation.PushAsync(new PerfilPage());

//                    return; 
//                }
//            }

//            string destino = !esDomicilio ? $"Mesa {_mesa.Numero}" : "Domicilio";
//            string tipoEntrega = PkrEntrega.SelectedItem?.ToString() ?? (esDomicilio ? "Domicilio" : "Mesa");

//            if (esClienteDomicilio)
//            {
//                PedidoTemporal.Notas = EntNotasCliente.Text?.Trim() ?? "";
//                PedidoTemporal.Direccion = EntDomicilioCliente.Text?.Trim() ?? "";
//            }

//            if (esClienteDomicilio && string.IsNullOrWhiteSpace(PedidoTemporal.Direccion))
//            {
//                await DisplayAlert("Atención", "Agrega tu dirección de entrega.", "OK");
//                return;
//            }

//            decimal costoEnvio = esClienteDomicilio ? 20m : 0m;
//            decimal subtotal = _carritoController.CalcularTotal();
//            decimal total = subtotal + costoEnvio;

//            bool confirmar = await DisplayAlert("Confirmar pedido",
//                $"{destino} — {EntNombreCliente.Text}\n" +
//                $"Subtotal: {subtotal:C2}" +
//                (costoEnvio > 0 ? $"\nEnvío: {costoEnvio:C2}" : "") +
//                $"\nTotal: {total:C2}",
//                "✅ Confirmar", "Cancelar");

//            if (!confirmar) return;

//            OverlayCarga.IsVisible = true;
//            Loader.IsRunning = true;

//            try
//            {
//                bool exito = await _ordenService.GuardarOrdenAsync(
//                    items: CarritoController.Items.ToList(),
//                    mesaId: _mesa?.Id,
//                    nombreCliente: EntNombreCliente.Text.Trim(),
//                    tipoEntrega: tipoEntrega,
//                    clienteId: PedidoTemporal.IdCliente > 0 ? PedidoTemporal.IdCliente : null,
//                    notas: PedidoTemporal.Notas,
//                    costoEnvio: costoEnvio
//                );

//                if (exito)
//                {
//                    LimpiarDatosTemporales();
//                    CarritoController.Items.Clear();
//                    await DisplayAlert("¡Éxito!", "Orden enviada a cocina. 🍔", "OK");
//                    EnviarNotificacion();
//                    seleccionMesaPage.CargarMesasAsync();
//                    await Navigation.PopToRootAsync();
//                }
//                else
//                {
//                    await DisplayAlert("Error", "No se pudo guardar. Revisa internet.", "OK");
//                }
//            }
//            catch (Exception ex)
//            {
//                await DisplayAlert("Error Crítico", ex.Message, "OK");
//            }
//            finally
//            {
//                OverlayCarga.IsVisible = false;
//                Loader.IsRunning = false;
//            }
//        }

//        public void EnviarNotificacion()
//        {
//            var request = new NotificationRequest()
//            {
//                NotificationId = 1332,
//                Title = "Pedido confirmado 🍔",
//                Description = "Tu orden fue enviada correctamente"
//            };

//            LocalNotificationCenter.Current.Show(request);
//        }
//        private void LimpiarDatosTemporales()
//        {
//            PedidoTemporal.IdCliente = null;
//            PedidoTemporal.NombreCliente = "";
//            PedidoTemporal.Notas = "";
//            PedidoTemporal.Direccion = "";
//        }
//    }
//}

using RestauranteNoseCual.Models;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Services;
using Plugin.LocalNotification;

namespace RestauranteNoseCual.View
{
    public partial class CarritoPage : ContentPage
    {
        private readonly OrdenService _ordenService = new();
        private readonly CarritoController _carritoController = new();
        private readonly ClienteService _clienteService = new();
        private readonly Mesa? _mesa;
        SeleccionMesaPage seleccionMesaPage = new SeleccionMesaPage();

        public CarritoPage(Mesa? mesa)
        {
            InitializeComponent();
            _mesa = mesa;

            if (SesionService.HaySesionActiva())
            {
                var (correo, nombre) = SesionService.ObtenerSesion();
                string rol = SesionService.ObtenerRol();

                if (rol == "Cliente")
                {
                    EntNombreCliente.Text = nombre;
                    PedidoTemporal.NombreCliente = nombre;
                    PedidoTemporal.IdCliente = Preferences.Get("sesion_id", (long)0);
                }
            }

            if (_mesa == null)
            {
                PkrEntrega.SelectedItem = "Domicilio";
                PkrEntrega.IsEnabled = false;
                if (string.IsNullOrEmpty(EntNombreCliente.Text))
                    EntNombreCliente.Text = PedidoTemporal.NombreCliente;
            }
            else
            {
                PkrEntrega.SelectedItem = "Mesa";
            }

            ActualizarTotal();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            string rol = SesionService.ObtenerRol();

            if (rol == "Cliente" && _mesa == null)
            {
                PanelDomicilio.IsVisible = true;
                await CargarDatosClienteAsync();
            }

            RefrescarGrid();
            ActualizarTotal();
        }

        private async Task CargarDatosClienteAsync()
        {
            try
            {
                var (correo, _) = SesionService.ObtenerSesion();
                var cliente = await _clienteService.ObtenerPorCorreoAsync(correo);

                if (cliente != null)
                {
                    EntDomicilioCliente.Text = cliente.Domicilio;
                    EntNotasCliente.Text = cliente.Notas;
                    PedidoTemporal.Notas = cliente.Notas;
                    PedidoTemporal.Direccion = cliente.Domicilio;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando datos cliente: {ex.Message}");
            }
        }

        private void ActualizarTotal()
        {
            if (LblTotal == null) return;

            bool esDomicilio = _mesa == null && SesionService.ObtenerRol() == "Cliente";
            decimal subtotal = _carritoController.CalcularTotal();
            decimal envio = esDomicilio ? 20m : 0m;
            decimal total = subtotal + envio;

            LblTotal.Text = total.ToString("C2");

            if (FilaEnvio != null)
                FilaEnvio.IsVisible = esDomicilio;
        }

        private void RefrescarGrid()
        {
            bool hayItems = CarritoController.Items != null && CarritoController.Items.Count > 0;

            GridCarrito.IsVisible = hayItems;
            LblCarritoVacio.IsVisible = !hayItems;

            if (hayItems)
            {
                GridCarrito.ItemsSource = null;
                GridCarrito.ItemsSource = CarritoController.Items;

                // Header (36) + filas + padding extra (10) para que no se corte
                GridCarrito.HeightRequest = 36 + (CarritoController.Items.Count * 80) + 10;
            }
        }

        private void OnSumarCantidad(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not CarritoItem item) return;

            item.Cantidad++;

            RefrescarGrid();
            ActualizarTotal();
        }

        private async void OnRestarCantidad(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not CarritoItem item) return;

            if (item.Cantidad <= 1)
            {
                bool confirmar = await DisplayAlert(
                    "Eliminar producto",
                    $"¿Quitar '{item.Producto.Nombre}' del carrito?",
                    "Sí", "No");

                if (!confirmar) return;

                _carritoController.EliminarItem(item);
            }
            else
            {
                item.Cantidad--;
            }

            RefrescarGrid();
            ActualizarTotal();
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

            bool esDomicilio = _mesa == null;
            string rol = SesionService.ObtenerRol();
            bool esClienteDomicilio = esDomicilio && rol == "Cliente";

            if (esClienteDomicilio)
            {
                var (correo, _) = SesionService.ObtenerSesion();
                var cliente = await _clienteService.ObtenerPorCorreoAsync(correo);

                bool perfilIncompleto = cliente == null
                    || string.IsNullOrWhiteSpace(cliente.Telefono)
                    || string.IsNullOrWhiteSpace(cliente.Domicilio);

                if (perfilIncompleto)
                {
                    bool irAPerfil = await DisplayAlert(
                        "Perfil incompleto",
                        "Necesitas teléfono y domicilio para hacer un pedido a domicilio.\n¿Ir a tu perfil ahora?",
                        "Sí, completar", "Cancelar");

                    if (irAPerfil)
                        await Navigation.PushAsync(new PerfilPage());

                    return;
                }
            }

            string destino = !esDomicilio ? $"Mesa {_mesa!.Numero}" : "Domicilio";
            string tipoEntrega = PkrEntrega.SelectedItem?.ToString() ?? (esDomicilio ? "Domicilio" : "Mesa");

            if (esClienteDomicilio)
            {
                PedidoTemporal.Notas = EntNotasCliente.Text?.Trim() ?? "";
                PedidoTemporal.Direccion = EntDomicilioCliente.Text?.Trim() ?? "";
            }

            if (esClienteDomicilio && string.IsNullOrWhiteSpace(PedidoTemporal.Direccion))
            {
                await DisplayAlert("Atención", "Agrega tu dirección de entrega.", "OK");
                return;
            }

            decimal costoEnvio = esClienteDomicilio ? 20m : 0m;
            decimal subtotal = _carritoController.CalcularTotal();
            decimal total = subtotal + costoEnvio;

            bool confirmar = await DisplayAlert("Confirmar pedido",
                $"{destino} — {EntNombreCliente.Text}\n" +
                $"Subtotal: {subtotal:C2}" +
                (costoEnvio > 0 ? $"\nEnvío: {costoEnvio:C2}" : "") +
                $"\nTotal: {total:C2}",
                "✅ Confirmar", "Cancelar");

            if (!confirmar) return;

            OverlayCarga.IsVisible = true;
            Loader.IsRunning = true;

            try
            {
                bool exito = await _ordenService.GuardarOrdenAsync(
                    items: CarritoController.Items.ToList(),
                    mesaId: _mesa?.Id,
                    nombreCliente: EntNombreCliente.Text.Trim(),
                    tipoEntrega: tipoEntrega,
                    clienteId: PedidoTemporal.IdCliente > 0 ? PedidoTemporal.IdCliente : null,
                    notas: PedidoTemporal.Notas,
                    costoEnvio: costoEnvio
                );

                if (exito)
                {
                    LimpiarDatosTemporales();
                    CarritoController.Items.Clear();
                    await DisplayAlert("¡Éxito!", "Orden enviada a cocina. 🍔", "OK");
                    EnviarNotificacion();
                    seleccionMesaPage.CargarMesasAsync();
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

        public void EnviarNotificacion()
        {
            var request = new NotificationRequest()
            {
                NotificationId = 1332,
                Title = "Pedido confirmado 🍔",
                Description = "Tu orden fue enviada correctamente"
            };

            LocalNotificationCenter.Current.Show(request);
        }

        private void LimpiarDatosTemporales()
        {
            PedidoTemporal.IdCliente = null;
            PedidoTemporal.NombreCliente = "";
            PedidoTemporal.Notas = "";
            PedidoTemporal.Direccion = "";
        }
    }
}
