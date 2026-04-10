using Microsoft.Maui.Controls;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View
{
    public partial class OrdenDetallePage : ContentPage
    {
        private readonly OrdenService _ordenService = new();
        private readonly Pedido _pedido;

        public OrdenDetallePage(Pedido pedido)
        {
            InitializeComponent();
            _pedido = pedido;

            GridDetalle.QueryRowHeight += (sender, e) =>
            {
                if (e.RowIndex > 0)
                {
                    e.Height = 52;
                    e.Handled = true;
                }
            };

            CargarDetalleAsync();
        }

        private async void CargarDetalleAsync()
        {
            LblFolio.Text = $"#{_pedido.Id}";
            LblCliente.Text = _pedido.NombreCliente;
            LblTipo.Text = _pedido.TipoEntrega;
            LblTotal.Text = _pedido.Total.ToString("C2");

            
            if (_pedido.TipoEntrega == "Domicilio" || _pedido.MesaId == null)
            {
                LblMesa.IsVisible = false;
            }
            else
            {
                LblMesa.IsVisible = true;
                LblMesa.Text = $"Mesa {_pedido.MesaId}";
            }

            string rol = SesionService.ObtenerRol();
            bool esPersonalInterno = rol == "Admin" || rol == "Mesero";
            if(esPersonalInterno && _pedido.Estado != "Pagada")
            {
                BtnCobrar.IsVisible = true;
                if (LblTipo.Text == "Domicilio") 
                {
  
                    BtnCobrar.Text = "Pagar pedido";

                }

            }
            var detalle = await _ordenService.ObtenerDetalleAsync(_pedido.Id);
            GridDetalle.ItemsSource = detalle;
        }

        private async void OnCobrarClicked(object sender, EventArgs e)
        {
            if(LblTipo.Text == "Domicilio")
            {
                 bool confirmar = await DisplayAlert(
                " Pagar pedido",
                $"¿Confirmas el pago de {_pedido.Total:C2} para el pedido a domicilio?\nSe marcará como pagado.",
                "Sí, pagar", "Cancelar");
                if (!confirmar) return;
                BtnCobrar.IsEnabled = false;
                bool exito = await _ordenService.CerrarOrdenAsync(_pedido.Id,_pedido.MesaId);
                if (exito)
                {
                    await DisplayAlert("Listo", "Pedido a domicilio pagado", "OK");
                    await Navigation.PopAsync(); 
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo completar el pago", "OK");
                    BtnCobrar.IsEnabled = true;
                }
            }
            else
            {
                bool confirmar = await DisplayAlert(
                " Cobrar",
                $"¿Confirmas el cobro de {_pedido.Total:C2}?\nSe liberará la Mesa {_pedido.MesaId}.",
                "Sí, cobrar", "Cancelar");

                if (!confirmar) return;

                BtnCobrar.IsEnabled = false;

                bool exito = await _ordenService.CerrarOrdenAsync(_pedido.Id, _pedido.MesaId);

                if (exito)
                {
                    await DisplayAlert("Listo", "Orden cobrada y mesa liberada", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo completar el cobro", "OK");
                    BtnCobrar.IsEnabled = true;
                }
            }
                
        }
    }
}