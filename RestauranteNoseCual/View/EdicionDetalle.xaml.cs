namespace RestauranteNoseCual.View;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;

public partial class EdicionDetalle : ContentPage
{
    string Fotografia;
    string nuevaRutaImagen = null;
    decimal Precio;
    long Id;
    bool Disponible; // ?? nuevo
    MenuController menuController = new MenuController();




    public EdicionDetalle(string nombre, decimal precio, string descripcion,
                          string fotografia, string categoria, long id, bool disponible)
    {
        InitializeComponent();
        txNombre.Text = nombre;
        txPrecio.Text = precio.ToString();
        txDescripcion.Text = descripcion;
        cmCategoria.SelectedItem = categoria;
        ImgProducto.Source = fotografia;
        Fotografia = fotografia;
        Id = id;
        Disponible = disponible;

        // ?? Inicializar el switch con el valor actual
        swDisponible.IsToggled = disponible;
    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var file = await FilePicker.PickAsync();
        if (file != null)
        {
            nuevaRutaImagen = file.FullPath;
            ImgProducto.Source = ImageSource.FromFile(file.FullPath);
        }
    }
    private async void Guardar_Clicked(object sender, EventArgs e)
    {
        AltaMenu producto = new()
        {
            Nombre = txNombre.Text,
            Precio = decimal.Parse(txPrecio.Text),
            Descripcion = txDescripcion.Text,
            Categoria = cmCategoria.SelectedItem?.ToString(),
            Id = Id,
            Fotografia = nuevaRutaImagen ?? Fotografia,
            Disponible = swDisponible.IsToggled // ?? nuevo
        };

        try
        {
            var resultado = await menuController.ActualizarProducto(producto);
            if (resultado != null)
            {
                await DisplayAlert("Éxito", "Producto actualizado correctamente", "OK");
                await Navigation.PushAsync(new EditarProducto());
            }
            else
            {
                await DisplayAlert("Error", "No se actualizó el producto", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
    }


    private void Agregar_Clicked(object sender, EventArgs e)
    {

    }
}