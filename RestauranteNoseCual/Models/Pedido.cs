using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteNoseCual.Models
{
    [Table("Orden")]
    public class Pedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("MesaId")]
        public long MesaId { get; set; }

        [Column("Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Column("Total")]
        public decimal Total { get; set; }

        [Column("FechaHora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}
