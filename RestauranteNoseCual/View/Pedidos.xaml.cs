using Microsoft.Maui.Controls;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;

namespace RestauranteNoseCual.View
{
    public partial class Pedidos : ContentPage
    {
        private readonly PedidosController _controller = new();

        public Pedidos()
        {
            InitializeComponent();
            GridPedidos.ItemsSource = _controller.ListaPedidos;
            FiltroPicker.SelectedIndex = 0;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _controller.CargarPedidosAsync();
            RefrescarGrid();
        }

        //Filtro
        private async void OnFiltroChanged(object sender, EventArgs e)
        {
            string filtro = FiltroPicker.SelectedItem?.ToString() ?? "Todos";
            var filtrados = await _controller.FiltrarPorTipoAsync(filtro);
            GridPedidos.ItemsSource = null;
            GridPedidos.ItemsSource = filtrados;
        }

        //Ver detalle
        private async void OnVerDetalleClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Pedido pedido)
                await Navigation.PushAsync(new OrdenDetallePage(pedido));
        }

        private void RefrescarGrid()
        {
            GridPedidos.ItemsSource = null;
            GridPedidos.ItemsSource = _controller.ListaPedidos;
        }
    }
}