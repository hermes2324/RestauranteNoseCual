using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;

namespace RestauranteNoseCual.Services
{
    public class AltaMenuService
    {
        private readonly Supabase.Client _supabase = Conexion.Supabase;

        public async Task<List<AltaMenu>> ObtenerTodosAsync()
        {
            var resultado = await _supabase
                .From<AltaMenu>()
                .Get();
            return resultado.Models;
        }

        public async Task<List<AltaMenu>> ObtenerPorCategoriaAsync(string categoria)
        {
            var resultado = await _supabase
                .From<AltaMenu>()
                .Where(p => p.Categoria == categoria)
                .Get();
            return resultado.Models;
        }

        public async Task<AltaMenu> AgregarProductoAsync(AltaMenu alta)
        {
            var resultado = await _supabase
                .From<AltaMenu>()
                .Insert(alta);
            return resultado.Models.FirstOrDefault();
        }
        public async Task<AltaMenu> ActualizarProductoAsync(AltaMenu alta)
        {
            var resultado = await _supabase
                .From<AltaMenu>()
                .Where(p => p.Id == alta.Id)
                .Update(alta);
            return resultado.Models.FirstOrDefault();
        }
        public async Task EliminarProductoAsync(long id)
        {
            await _supabase
                .From<AltaMenu>()
                .Where(p => p.Id == id)
                .Delete();
        }
    }
}
