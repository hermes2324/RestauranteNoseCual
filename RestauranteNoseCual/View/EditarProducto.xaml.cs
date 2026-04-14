namespace RestauranteNoseCual.View;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;

public partial class EditarProducto : ContentPage
{
    private readonly MenuController _menuController = new();
    

    public EditarProducto()
	{
		InitializeComponent();
        CargarProductosAsync("Todos");
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        CargarProductosAsync("Todos");
    }
    public async void CargarProductosAsync(string categoria)
    {
        Cargando.IsVisible = true;
        Cargando.IsRunning = true;

        List<AltaMenu> productos;

        if (categoria == "Todos")
            productos = await _menuController.ObtenerTodosAsync();
        else
            productos = await _menuController.ObtenerPorCategoriaAsync(categoria);

        ListaProductos.ItemsSource = productos;

        Cargando.IsVisible = false;
        Cargando.IsRunning = false;
    }

   
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }

    //Boton para editar producto
    private async void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.BindingContext as AltaMenu;
        if (producto == null) return;

        await Navigation.PushAsync(new EdicionDetalle(
            producto.Nombre,
            producto.Precio,
            producto.Descripcion,
            producto.Fotografia,
            producto.Categoria,
            producto.Id,
            producto.Disponible // ?? nuevo parámetro
        ));

        //CargarProductosAsync("Todos");
    }

    //Boton para eliminar producto
    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.CommandParameter as AltaMenu;

        if (producto == null)
            return;

        bool confirmacion = await DisplayAlert(
            "Confirmar",
            $"¿Eliminar {producto.Nombre}?",
            "Sí",
            "Cancelar"
        );

        if (!confirmacion)
            return;

        try
        {
        await _menuController.EliminarProducto(producto.Id);

            await DisplayAlert("Éxito", "Producto eliminado", "OK");

            // Recargar lista
            CargarProductosAsync("Todos");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
        }
    }
}