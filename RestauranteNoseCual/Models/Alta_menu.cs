using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RestauranteNoseCual.Models
{
    [Table("Alta_Menu")]
    public class AltaMenu : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Descripción")]
        public string Descripcion { get; set; }

        [Column("Precio")]
        public decimal Precio { get; set; }

        [Column("Fotografia")]
        public string Fotografia { get; set; }

        [Column("Categoria")]
        public string Categoria { get; set; }
    }
}