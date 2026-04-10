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

        //public async Task<bool> GuardarOrdenAsync(List<CarritoItem> items, long mesaId,
        //                                    string nombreCliente, string tipoEntrega)
        //{
        //    try
        //    {
        //        decimal totalOrden = items.Sum(x => x.Subtotal);
        //        var nuevaOrden = new Pedido
        //        {
        //            MesaId = mesaId,
        //            NombreCliente = nombreCliente,
        //            TipoEntrega = tipoEntrega,
        //            Total = totalOrden,
        //            Estado = "Pendiente",
        //            FechaHora = DateTime.Now
        //        };


        //        await _supabase.From<Pedido>().Insert(nuevaOrden);


        //        var busqueda = await _supabase.From<Pedido>()
        //            .Where(p => p.MesaId == mesaId && p.Estado == "Pendiente")
        //            .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
        //            .Limit(1)
        //            .Get();

        //        var ordenReal = busqueda.Models.FirstOrDefault();
        //        if (ordenReal == null) return false;

        //        var detalles = items.Select(item => new DetallePedido
        //        {
        //            OrdenId = ordenReal.Id,
        //            ProductoId = item.Producto.Id,
        //            Cantidad = item.Cantidad,
        //            Subtotal = item.Subtotal
        //        }).ToList();

        //        await _supabase.From<DetallePedido>().Insert(detalles);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return false;
        //    }
        //}

        // Todos los pedidos
        public async Task<bool> GuardarOrdenAsync(
    List<CarritoItem> items,
    long? mesaId,
    string nombreCliente,
    string tipoEntrega,
    long? clienteId = null,
    string? notas = null,
    decimal costoEnvio = 0)
        {
            try
            {
                decimal totalOrden = items.Sum(x => x.Subtotal) + costoEnvio;

                var nuevaOrden = new Pedido
                {
                    MesaId = mesaId,
                    NombreCliente = nombreCliente,
                    TipoEntrega = tipoEntrega,
                    Total = totalOrden,
                    Estado = "En preparación",
                    FechaHora = DateTime.Now,
                    ClienteId = clienteId,
                    Notas = notas,
                    CostoEnvio = costoEnvio
                };

                var respuesta = await _supabase.From<Pedido>().Insert(nuevaOrden);
                var ordenReal = respuesta.Models.FirstOrDefault();

                if (ordenReal == null)
                {
                    var query = _supabase.From<Pedido>()
                        .Where(p => p.NombreCliente == nombreCliente && p.Estado == "En preparación");

                    if (mesaId.HasValue)
                        query = query.Where(p => p.MesaId == mesaId.Value);

                    var busqueda = await query
                        .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
                        .Limit(1)
                        .Get();

                    ordenReal = busqueda.Models.FirstOrDefault();
                }

                if (ordenReal == null) return false;

                var detalles = items.Select(item => new DetallePedido
                {
                    OrdenId = ordenReal.Id,
                    ProductoId = item.Producto.Id,
                    Cantidad = item.Cantidad,
                    Subtotal = item.Subtotal
                }).ToList();

                await _supabase.From<DetallePedido>().Insert(detalles);

                
                if (clienteId.HasValue && !string.IsNullOrWhiteSpace(notas))
                {
                    await _supabase
                        .From<Cliente>()
                        .Where(c => c.Id == clienteId.Value)
                        .Set(c => c.Notas, notas)
                        .Update();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GuardarOrden] {ex.Message}");
                return false;
            }
        }
        public async Task<List<Pedido>> ObtenerPedidosAsync()
        {
            try
            {
                var resultado = await _supabase.From<Pedido>()
                    .Where(p => p.Estado != "Pagado") 
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

        //  Filtrar por tipo de entrega
        public async Task<List<Pedido>> ObtenerPorTipoAsync(string tipoEntrega)
        {
            try
            {
                var resultado = await _supabase.From<Pedido>()
                    .Where(p => p.TipoEntrega == tipoEntrega)
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

        //  Obtener detalle de una orden
        public async Task<List<DetallePedido>> ObtenerDetalleAsync(long ordenId)
        {
            try
            {

                var resultado = await _supabase.From<DetallePedido>()
                    .Where(d => d.OrdenId == ordenId)
                    .Get();

                var detalles = resultado.Models;


                foreach (var detalle in detalles)
                {
                    var productoResult = await _supabase.From<AltaMenu>()
                        .Where(p => p.Id == detalle.ProductoId)
                        .Get();

                    var producto = productoResult.Models.FirstOrDefault();
                    if (producto != null)
                    {
                        detalle.NombreProducto = producto.Nombre;
                        detalle.PrecioUnitario = producto.Precio; 
                    }
                }

                return detalles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DetallePedido>();
            }
        }

        public async Task<bool> ActualizarEstadoPedido(Pedido pedido)
        {
            try
            {
                await _supabase.From<Pedido>()
                    .Where(p => p.Id == pedido.Id)
                    .Set(p => p.Estado, pedido.Estado)
                    .Update();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        // Cerrar orden y liberar mesa
        //public async Task<bool> CerrarOrdenAsync(long ordenId, long mesaId)
        //{
        //    try
        //    {

        //        await _supabase.From<Pedido>()
        //            .Where(p => p.Id == ordenId)
        //            .Set(p => p.Estado, "Pagada")
        //            .Update();


        //        await _supabase.From<Mesa>()
        //            .Where(m => m.Id == mesaId)
        //            .Set(m => m.Estado, "Libre")
        //            .Update();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return false;
        //    }
        //}
        public async Task<List<Pedido>> ObtenerPorClienteAsync(long clienteId)
        {
            var result = await _supabase
                .From<Pedido>()
                .Where(o => o.ClienteId == clienteId)
                .Get();
            return result.Models;
        }
        public async Task<bool> CerrarOrdenAsync(long ordenId, long? mesaId) 
        {
            try
            {
               
                await _supabase.From<Pedido>()
                    .Where(p => p.Id == ordenId)
                    .Set(p => p.Estado, "Pagada")
                    .Update();

               
                if (mesaId.HasValue)
                {
                    await _supabase.From<Mesa>()
                        .Where(m => m.Id == mesaId.Value)
                        .Set(m => m.Estado, "Libre")
                        .Update();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar orden: {ex.Message}");
                return false;
            }
        }
    }
}
