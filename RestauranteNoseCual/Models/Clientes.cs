using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace RestauranteNoseCual.Models
{
    [Table("Clientes")]
    public class Clientes : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("correo")]
        public string Correo { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
