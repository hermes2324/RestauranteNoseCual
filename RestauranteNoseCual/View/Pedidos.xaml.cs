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
        //Lista de los posibles estados 
        public List<string> ListaEstados { get; set; } = new()
        {
            "En preparación",
            "En proceso de entrega",
            "Entregado",
            "Cancelado por el cliente"
        };

        //private async void GridPedidos_CurrentCellEndEdit(object sender, Syncfusion.Maui.DataGrid.DataGridCurrentCellEndEditEventArgs e)
        //{
        //    var grid = sender as Syncfusion.Maui.DataGrid.SfDataGrid;

        //    if (grid == null) return;

        //    // 🔥 Obtener índice de fila
        //    int rowIndex = e.RowColumnIndex.RowIndex;

        //    // ⚠️ Restar 1 porque el header cuenta como fila 0
        //    var record = grid.View.Records[rowIndex - 1];

        //    var pedido = record.Data as Pedido;

        //    if (pedido == null) return;

        //    // 🔥 Aquí ya tienes el estado actualizado
        //    string nuevoEstado = pedido.Estado;

        //    await _controller.ActualizarEstadoAsync(pedido, nuevoEstado);
        //}

        //esta opcion es por si no jala el de syncfusion, pero el de syncfusion es mas directo porque ya te da el pedido actualizado
        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (picker?.BindingContext is Pedido pedido)
            {
                string nuevoEstado = pedido.Estado;

                await _controller.ActualizarEstadoAsync(pedido, nuevoEstado);
            }
        }
    }
}