using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;

namespace RestauranteNoseCual.Services
{
    public class MesaService
    {
        private readonly Supabase.Client _supabase = Conexion.Supabase;

        // Obtener todas las mesas
        public async Task<List<Mesa>> ObtenerTodasAsync()
        {
            var resultado = await _supabase
                .From<Mesa>()
                .Order(m => m.Numero, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();
            return resultado.Models;
        }

        // Cambiar estado de mesa
        public async Task CambiarEstadoAsync(long mesaId, string estado)
        {
            await _supabase
                .From<Mesa>()
                .Where(m => m.Id == mesaId)
                .Set(m => m.Estado, estado)
                .Update();
        }
    }
}