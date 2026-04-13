using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[Table("Orden")]
public class Pedido : BaseModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string estado = "Pendiente";

    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("MesaId")]
    public long? MesaId { get; set; }

    [Column("NombreCliente")]
    public string NombreCliente { get; set; } = string.Empty;

    [Column("Estado")]
    //public string Estado
    //{
    //    get => estado;
    //    set
    //    {
    //        if (estado != value)
    //        {
    //            estado = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}
    public string Estado
    {
        get => estado;
        set
        {
            if (estado == value) return;
            estado = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Estado)));
        }
    }

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

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}