using RestauranteNoseCual.Models;
using RestauranteNoseCual.Controllers;

namespace RestauranteNoseCual.View;

public partial class AgregarPage : ContentPage
{
    private readonly MenuController _controller = new MenuController();
    string rutaImagenSeleccionada = "";

    public AgregarPage()
    {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                rutaImagenSeleccionada = photo.FullPath;
                var stream = await photo.OpenReadAsync();
                ImgProducto.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar la imagen: {ex.Message}", "OK");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        AltaMenu alta = new AltaMenu
        {
            Nombre = txNombre.Text,
            Descripcion = txDescripcion.Text,
            Precio = decimal.TryParse(txPrecio.Text, out decimal precio) ? precio : 0,
            Categoria = cmCategoria.SelectedItem?.ToString(),
            Fotografia = rutaImagenSeleccionada,
            Disponible = swDisponible.IsToggled // ?? nuevo
        };

        try
        {
            if (string.IsNullOrEmpty(alta.Nombre) ||
                string.IsNullOrEmpty(alta.Descripcion) ||
                alta.Precio == 0 ||
                string.IsNullOrEmpty(alta.Categoria))
            {
                await DisplayAlert("Error", "Por favor, complete todos los campos antes de agregar el producto.", "OK");
                return;
            }

            await _controller.AgregarProducto(alta);
            await DisplayAlert("Éxito", "Producto agregado correctamente", "OK");

            // Limpiar campos
            txNombre.Text = "";
            txDescripcion.Text = "";
            txPrecio.Text = "";
            cmCategoria.SelectedIndex = -1;
            ImgProducto.Source = null;
            swDisponible.IsToggled = true; // ?? resetear switch
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo agregar el producto: {ex.Message}", "OK");
        }
    }
}