using Supabase;

namespace RestauranteNoseCual.Base_de_Datos
{
    public class Conexion
    {
        private static string url = "https://tkxlodmhhrqkmbndqion.supabase.co";
        private static string key = "sb_publishable_PsNyTIOM8VoHpzy263HcjA_OmpokRsq";

        private static Supabase.Client _supabase;

        public static Supabase.Client Supabase
        {
            get
            {
                if (_supabase == null)
                {
                    var options = new SupabaseOptions
                    {
                        AutoConnectRealtime = true
                    };
                    _supabase = new Supabase.Client(url, key, options);
                }
                return _supabase;
            }
        }
    }
}
