//using RestauranteNoseCual.Base_de_Datos;
//using RestauranteNoseCual.Models;
//using RestauranteNoseCual.Services;


//namespace RestauranteNoseCual.Controllers
//{
//    public class LoginController
//    {
//        private readonly ClienteService _clienteService = new();
//        private readonly AutentificacionGoogle _authService = new();

//        public async Task<(bool exito, string mensaje, Cliente? cliente)> LoginGoogleAsync()
//        {
//            try
//            {
//                var googleUser = await _authService.SignInAsync();
//                if (googleUser == null)
//                    return (false, "No se pudo iniciar sesión con Google", null);

//                var cliente = new Cliente
//                {
//                    Nombre = googleUser.Name,
//                    Correo = googleUser.Email,
//                    Contrasena = string.Empty
//                };

//                var clienteGuardado = await _clienteService.GuardarClienteAsync(cliente);


//                SesionService.GuardarSesion(
//                    clienteGuardado.Id,
//                    clienteGuardado.Correo,
//                    clienteGuardado.Nombre,
//                    clienteGuardado.Rol  
//                );

//                return (true, $"Bienvenido, {clienteGuardado.Nombre}", clienteGuardado);
//            }
//            catch (Exception ex)
//            {
//                return (false, ex.Message, null);
//            }
//        }


//        public async Task<(bool exito, string mensaje, Cliente? cliente)> LoginManualAsync(string correo, string contrasena)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
//                    return (false, "Ingresa correo y contraseña", null);

//                var cliente = await _clienteService.ValidarLoginAsync(correo, contrasena);

//                if (cliente == null)
//                    return (false, "Correo o contraseña incorrectos", null);


//                SesionService.GuardarSesion(
//                    cliente.Id,     
//                    cliente.Correo,  
//                    cliente.Nombre,  
//                    cliente.Rol      
//                );

//                return (true, $"Bienvenido, {cliente.Nombre}", cliente);
//            }
//            catch (Exception ex)
//            {
//                return (false, ex.Message, null);
//            }
//        }
//    }
//}
//El de arriba si funciona, el de aqui abajo no se, es una prueba para agregar el registro del token FCM al iniciar sesión, pero no se si es correcto o si falta algo, por eso lo dejo comentado para que lo revises, si es correcto lo puedes descomentar y si no pues me dices que le falta o que esta mal, gracias
using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.Controllers
{
    public class LoginController
    {
        private readonly ClienteService _clienteService = new();
        private readonly AutentificacionGoogle _authService = new();
        private readonly NotificationService _notificationService = new(); // 👈

        public async Task<(bool exito, string mensaje, Cliente? cliente)> LoginGoogleAsync()
        {
            try
            {
                var googleUser = await _authService.SignInAsync();
                if (googleUser == null)
                    return (false, "No se pudo iniciar sesión con Google", null);

                var cliente = new Cliente
                {
                    Nombre = googleUser.Name,
                    Correo = googleUser.Email,
                    Contrasena = string.Empty
                };

                var clienteGuardado = await _clienteService.GuardarClienteAsync(cliente);

                SesionService.GuardarSesion(
                    clienteGuardado.Id,
                    clienteGuardado.Correo,
                    clienteGuardado.Nombre,
                    clienteGuardado.Rol
                );

                // 👇 Iniciar escucha realtime si es mesero
                if (clienteGuardado.Rol == "Mesero")
                {
                    if (Application.Current is App app)
                    {
                        app.IniciarRealtimeNotifications();
                    }
                }

                await _notificationService.RegistrarTokenAsync(); // 👈

                return (true, $"Bienvenido, {clienteGuardado.Nombre}", clienteGuardado);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool exito, string mensaje, Cliente? cliente)> LoginManualAsync(string correo, string contrasena)
        {
            try
            {
                if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
                    return (false, "Ingresa correo y contraseña", null);

                var cliente = await _clienteService.ValidarLoginAsync(correo, contrasena);
                if (cliente == null)
                    return (false, "Correo o contraseña incorrectos", null);

                SesionService.GuardarSesion(
                    cliente.Id,
                    cliente.Correo,
                    cliente.Nombre,
                    cliente.Rol
                );

                await _notificationService.RegistrarTokenAsync(); // 👈

                return (true, $"Bienvenido, {cliente.Nombre}", cliente);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}