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

        // Guardar sesión al hacer login
        public static void GuardarSesion(string correo, string nombre)
        {
            Preferences.Set(KEY_CORREO, correo);
            Preferences.Set(KEY_NOMBRE, nombre);
            Preferences.Set(KEY_ACTIVA, true);
        }

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
            Preferences.Set(KEY_ACTIVA, false);
        }
    }
}
