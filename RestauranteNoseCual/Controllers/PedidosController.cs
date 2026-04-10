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
        private readonly OrdenService _service = new();
        public ObservableCollection<Pedido> ListaPedidos { get; set; } = new();

        public async Task CargarPedidosAsync()
        {
            var pedidos = await _service.ObtenerPedidosAsync();
            ListaPedidos.Clear();
            foreach (var p in pedidos)
                ListaPedidos.Add(p);
        }

        // Filtrar por tipo
        public async Task<List<Pedido>> FiltrarPorTipoAsync(string tipo)
        {
            if (tipo == "Todos")
                return await _service.ObtenerPedidosAsync();
            return await _service.ObtenerPorTipoAsync(tipo);
        }

        public async Task ActualizarEstadoAsync(Pedido pedido, string nuevoEstado)
        {
            pedido.Estado = nuevoEstado;
            await _service.ActualizarEstadoPedido(pedido);
        }
    }
}
