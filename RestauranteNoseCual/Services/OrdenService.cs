using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Base_de_Datos;

namespace RestauranteNoseCual.Services
{
    public class OrdenService
    {
        private readonly Supabase.Client _supabase = Conexion.Supabase;

        public async Task<bool> GuardarOrdenAsync(List<CarritoItem> items, long mesaId)
        {
            try
            {
                
                decimal totalOrden = items.Sum(x => x.Subtotal);

                
                var nuevaOrden = new Pedido
                {
                    MesaId = mesaId,
                    Total = totalOrden,
                    Estado = "Pendiente"
                };

                var respuesta = await _supabase.From<Pedido>().Insert(nuevaOrden);
                var ordenReal = respuesta.Models.FirstOrDefault();

                if (ordenReal == null) return false;

                
                var detalles = items.Select(item => new DetallePedido
                {
                    OrdenId = ordenReal.Id,
                    ProductoId = item.Producto.Id,
                    Cantidad = item.Cantidad,
                    Subtotal = item.Subtotal
                }).ToList();

                
                await _supabase.From<DetallePedido>().Insert(detalles);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public async Task<List<Pedido>> ObtenerPedidosAsync()
        {
            try
            {
                var resultado = await _supabase.From<Pedido>()
                    .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();
                return resultado.Models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Pedido>();
            }
        }
    }
}
