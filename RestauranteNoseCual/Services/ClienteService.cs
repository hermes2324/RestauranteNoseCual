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

        //public async Task<Cliente?> BuscarPorTelefonoAsync(string telefono)
        //{
        //    var resultado = await _supabase
        //        .From<Cliente>()
        //        .Where(c => c.Telefono == telefono)
        //        .Get();
        //    return resultado.Models.FirstOrDefault();
        //}
        public async Task<Cliente?> BuscarPorTelefonoAsync(string telefono)
        {
            try
            {
                var resultadoCliente = await _supabase
                    .From<Cliente>()
                    .Where(c => c.Telefono == telefono)
                    .Get();

                var cliente = resultadoCliente.Models.FirstOrDefault();

                return cliente;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BuscarPorTelefono: {ex.Message}");
                return null;
            }
        }

        public async Task<Cliente> GuardarOActualizarAsync(Cliente cliente)
        {
            if (cliente.Id > 0)
            {
                // Actualiza domicilio si ya existe
                await _supabase.From<Cliente>()
                    .Where(c => c.Id == cliente.Id)
                    .Set(c => c.Domicilio, cliente.Domicilio)
                    .Set(c => c.Nombre, cliente.Nombre)
                    .Set(c => c.Notas, cliente.Notas)
                    .Update();
                return cliente;
            }
            else
            {
                var resultado = await _supabase.From<Cliente>().Insert(cliente);
                return resultado.Models.First();
            }
        }


    }
}