using Supabase;

namespace RestauranteNoseCual.Base_de_Datos
{
    public class Conexion
    {
        private static string url = "https://tkxlodmhhrqkmbndqion.supabase.co";
        private static string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRreGxvZG1oaHJxa21ibmRxaW9uIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQyODYyMDMsImV4cCI6MjA4OTg2MjIwM30.0N26CNraQ8R1oKva4Sc_m1aK2iMPNWtfhCLz8N_ZwbQ";

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
