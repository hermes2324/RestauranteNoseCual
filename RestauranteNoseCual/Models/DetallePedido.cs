using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestauranteNoseCual.Models
{
    [Table("OrdenDetalle")]
    public class DetallePedido : BaseModel
    {
        [Column("OrdenId")]
        public long OrdenId { get; set; }

        [Column("ProductoId")]
        public long ProductoId { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Subtotal")]
        public decimal Subtotal { get; set; }

        [JsonIgnore]
        public string NombreProducto { get; set; } = string.Empty;

        [JsonIgnore]
        public decimal PrecioUnitario { get; set; }
    }
}
