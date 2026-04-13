namespace RestauranteNoseCual.View;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;

public partial class EdicionDetalle : ContentPage
{
    string Fotografia; // ? aquí guardamos la original
    string nuevaRutaImagen = null; // ? nueva si el usuario cambia
    decimal Precio;
    EditarProducto editarProductoPage = new EditarProducto();
    long Id;

    MenuController menuController = new MenuController();

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var file = await FilePicker.PickAsync();

        if (file != null)
        {
            nuevaRutaImagen = file.FullPath;
            ImgProducto.Source = ImageSource.FromFile(file.FullPath);
        }
    }

    public EdicionDetalle(string nombre, decimal precio, string descripcion, string fotografia, string categoria, long id)
    {
        InitializeComponent();

        txNombre.Text = nombre;
        txPrecio.Text = precio.ToString();
        txDescripcion.Text = descripcion;
        cmCategoria.SelectedItem = categoria;
        ImgProducto.Source = fotografia;

        Fotografia = fotografia; // ? guardamos la original
        Id = id;
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

            Fotografia = nuevaRutaImagen ?? Fotografia
        };

        try
        {
            var resultado = await menuController.ActualizarProducto(producto);

            if (resultado != null)
            {
                await DisplayAlert("Éxito", "Producto actualizado correctamente", "OK");
                await Navigation.PushAsync(new EditarProducto());   
                //editarProductoPage.CargarProductosAsync("Todos");
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