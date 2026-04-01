using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.Controllers
{
    public class PedidosController
    {
        private readonly OrdenService _service = new OrdenService();
        public ObservableCollection<Pedido> ListaPedidos { get; set; } = new ObservableCollection<Pedido>();

        public async Task CargarPedidosAsync()
        {
            var pedidos = await _service.ObtenerPedidosAsync();
            ListaPedidos.Clear();
            foreach (var p in pedidos)
            {
                ListaPedidos.Add(p);
            }
        }
    }
}
