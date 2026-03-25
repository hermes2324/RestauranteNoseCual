using RestauranteNoseCual.Models;
using RestauranteNoseCual.Base_de_Datos;
using Supabase;

namespace RestauranteNoseCual.Services
{
    public class ClienteService
    {
        private readonly Client _supabase = Conexion.Supabase;

        public async Task<Cliente?> ObtenerPorCorreoAsync(string correo)
        {
            var resultado = await _supabase
                .From<Cliente>()
                .Where(c => c.Correo == correo)
                .Get();
            return resultado.Models.FirstOrDefault();
        }

        public async Task<Cliente> GuardarClienteAsync(Cliente cliente)
        {
            var existe = await ObtenerPorCorreoAsync(cliente.Correo);
            if (existe != null)
                return existe;

            var resultado = await _supabase
                .From<Cliente>()
                .Insert(cliente);
            return resultado.Models.First();
        }

        
        public async Task<Cliente?> ValidarLoginAsync(string correo, string contrasena)
        {
            var cliente = await ObtenerPorCorreoAsync(correo);
            if (cliente == null) return null;
            if (cliente.Contrasena != contrasena) return null;
            return cliente;
        }
    }
}