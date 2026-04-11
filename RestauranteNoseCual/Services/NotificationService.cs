using Plugin.Firebase.CloudMessaging;
using RestauranteNoseCual.Base_de_Datos;

namespace RestauranteNoseCual.Services
{
    public class NotificationService
    {
        private readonly Supabase.Client _supabase = Conexion.Supabase;

        public async Task RegistrarTokenAsync()
        {
            try
            {
                var rol = SesionService.ObtenerRol();
                if (rol != "Mesero") return;

                var id = SesionService.ObtenerIdCliente();
                if (id == 0) return;

                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                await _supabase
                    .From<Models.Cliente>()
                    .Where(c => c.Id == id)
                    .Set(c => c.FcmToken, token)
                    .Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotificationService] Error: {ex.Message}");
            }
        }
    }
}