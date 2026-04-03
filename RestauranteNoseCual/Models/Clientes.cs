using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace RestauranteNoseCual.Models
{
    [Table("Cliente")]
    public class Cliente : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Telefono")]
        public string Telefono { get; set; } 

        [Column("Domicilio")]
        public string Domicilio { get; set; }

        [Column("Correo")]
        public string Correo { get; set; }

        [Column("Contraseña")]
        public string Contrasena { get; set; }

        [JsonIgnore]
        public string UltimasNotas { get; set; } = string.Empty;

    }
}