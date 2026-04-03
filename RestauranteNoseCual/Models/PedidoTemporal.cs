namespace RestauranteNoseCual.Models
{
    public static class PedidoTemporal
    {
        public static string NombreCliente { get; set; } = string.Empty;
        public static string Telefono { get; set; } = string.Empty;
        public static string Direccion { get; set; } = string.Empty;
        public static string Notas { get; set; } = string.Empty;
        public static decimal CostoEnvio { get; set; } = 20;

        public static long? IdCliente { get; set; }
    }
}