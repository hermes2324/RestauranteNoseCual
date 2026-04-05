using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.Controllers
{
    public class RegistroController
    {
        private readonly ClienteService _clienteService = new();

        public async Task<(bool exito, string mensaje)> RegistrarAsync(
            string nombre, string telefono, string domicilio,
            string correo, string contrasena, string confirmar)
        {
            // Validaciones
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) ||
                string.IsNullOrEmpty(contrasena) || string.IsNullOrEmpty(telefono))
                return (false, "Todos los campos son obligatorios");

            if (contrasena != confirmar)
                return (false, "Las contraseñas no coinciden");

            if (contrasena.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres");

            // Verificar si el correo ya existe
            var existe = await _clienteService.ObtenerPorCorreoAsync(correo);
            if (existe != null)
                return (false, "Este correo ya está registrado");

            // Guardar cliente
            var nuevoCliente = new Cliente
            {
                Nombre = nombre,
                Telefono = telefono,
                Domicilio = domicilio,
                Correo = correo,
                Contrasena = contrasena,
                Rol = "Cliente"
            };

            await _clienteService.GuardarClienteAsync(nuevoCliente);
            return (true, "Cuenta creada exitosamente");
        }
    }
}