using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestauranteNoseCual.Models;

namespace RestauranteNoseCual.Controllers
{
    public class CarritoController
    {
        public static ObservableCollection<CarritoItem> Items { get; set; } = new();

        public void AgregarProducto(AltaMenu producto)
        {
            var existente = Items.FirstOrDefault(x => x.Producto.Id == producto.Id);

            if (existente != null)
            {
                existente.Cantidad++;
               
                var index = Items.IndexOf(existente);
                Items[index] = existente;
            }
            else
            {
                Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
            }
        }

        public void EliminarItem(CarritoItem item) => Items.Remove(item);

        public decimal CalcularTotal() => Items.Sum(x => x.Subtotal);
    }
}
