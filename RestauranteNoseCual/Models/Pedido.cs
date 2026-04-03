using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RestauranteNoseCual.Models
{
    [Table("Orden")]
    public class Pedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("MesaId")]
        public long? MesaId { get; set; }

        [Column("NombreCliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Column("Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Column("TipoEntrega")]
        public string TipoEntrega { get; set; } = "Mesa";

        [Column("Total")]
        public decimal Total { get; set; }

        [Column("FechaHora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

       
        [Column("ClienteId")]
        public long? ClienteId { get; set; }

        [Column("Notas")]
        public string? Notas { get; set; }

        [Column("CostoEnvio")]
        public decimal CostoEnvio { get; set; }
    }
}