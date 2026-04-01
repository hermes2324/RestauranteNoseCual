using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RestauranteNoseCual.Models
{
    [Table("Mesa")]
    public class Mesa : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("Numero")]
        public int Numero { get; set; }

        [Column("Estado")]
        public string Estado { get; set; } 
    }
}