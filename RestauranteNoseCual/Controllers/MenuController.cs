using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.Controllers
{
    public class MenuController
    {
        private readonly AltaMenuService _menuService = new();

        public async Task<List<AltaMenu>> ObtenerTodosAsync()
        {
            try { return await _menuService.ObtenerTodosAsync(); }
            catch { return new List<AltaMenu>(); }
        }

        public async Task<List<AltaMenu>> ObtenerPorCategoriaAsync(string categoria)
        {
            try { return await _menuService.ObtenerPorCategoriaAsync(categoria); }
            catch { return new List<AltaMenu>(); }
        }
    }
}