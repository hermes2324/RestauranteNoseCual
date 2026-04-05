using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteNoseCual.Services
{
    public static class SesionService
    {
        private const string KEY_CORREO = "sesion_correo";
        private const string KEY_NOMBRE = "sesion_nombre";
        private const string KEY_ACTIVA = "sesion_activa";
        private const string KEY_ROL = "sesion_rol";

        public static void GuardarSesion(long id, string correo, string nombre, string rol)
        {
            Preferences.Set("sesion_id", id);
            Preferences.Set("sesion_correo", correo);
            Preferences.Set("sesion_nombre", nombre);
            Preferences.Set(KEY_ROL, rol);
            Preferences.Set("sesion_activa", true);
        }
        public static long ObtenerIdCliente() => Preferences.Get("sesion_id", 0L);
        public static string ObtenerRol() => Preferences.Get(KEY_ROL, "Cliente");

        // Verificar si hay sesión activa
        public static bool HaySesionActiva()
        {
            return Preferences.Get(KEY_ACTIVA, false);
        }

        // Obtener datos de la sesión guardada
        public static (string correo, string nombre) ObtenerSesion()
        {
            string correo = Preferences.Get(KEY_CORREO, string.Empty);
            string nombre = Preferences.Get(KEY_NOMBRE, string.Empty);
            return (correo, nombre);
        }
        
        

        // Cerrar sesión (borrar todo)
        public static void CerrarSesion()
        {
            Preferences.Remove(KEY_CORREO);
            Preferences.Remove(KEY_NOMBRE);
            Preferences.Remove(KEY_ROL);
            Preferences.Set(KEY_ACTIVA, false);
        }
    }
}
