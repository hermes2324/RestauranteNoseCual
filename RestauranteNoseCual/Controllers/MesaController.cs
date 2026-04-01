using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.Controllers
{
    public class MesaController
    {
        private readonly MesaService _mesaService = new();

        public async Task<List<Mesa>> ObtenerMesasAsync()
        {
            try { return await _mesaService.ObtenerTodasAsync(); }
            catch { return new List<Mesa>(); }
        }

        public async Task OcuparMesaAsync(long mesaId)
        {
            try { await _mesaService.CambiarEstadoAsync(mesaId, "Ocupada"); }
            catch { }
        }

        public async Task LiberarMesaAsync(long mesaId)
        {
            try { await _mesaService.CambiarEstadoAsync(mesaId, "Libre"); }
            catch { }
        }
    }
}