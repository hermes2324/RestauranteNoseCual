using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestauranteNoseCual.Models
{
    [Table("Mesa")]
    public class Mesa : BaseModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("Numero")]
        public int Numero { get; set; }

        private string _estado;
        [Column("Estado")]
        public string Estado
        {
            get => _estado;
            set
            {
                if (_estado != value)
                {
                    _estado = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}