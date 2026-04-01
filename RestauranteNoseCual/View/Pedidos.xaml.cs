using RestauranteNoseCual.Controllers;

namespace RestauranteNoseCual.View
{
    public partial class Pedidos : ContentPage
    {
        private readonly PedidosController _controller = new PedidosController();

        public Pedidos()
        {
            InitializeComponent();
          
            GridPedidos.ItemsSource = _controller.ListaPedidos;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           
            await _controller.CargarPedidosAsync();
        }
    }
}