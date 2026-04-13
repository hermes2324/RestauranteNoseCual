using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class Pantalla_Principal : ContentPage
{
    private readonly AltaMenuService _menuService = new();
    Mesa _mesa;

    public Pantalla_Principal()
    {
        InitializeComponent();

        
        CarruselCombos.IndicatorView = IndicadorCombos;

        CargarUsuario();
        CargarDatosAsync();
    }

    private void CargarUsuario()
    {
        var datos = SesionService.ObtenerSesion();
        string nombre = datos.nombre;
        var hora = DateTime.Now.Hour;
        string saludo = hora < 12 ? "¡Buenos días! 🌅"
                      : hora < 18 ? "¡Buenas tardes! ☀️"
                      : "¡Buenas noches! 🌙";

        if (LblSaludo != null) LblSaludo.Text = saludo;
        if (LblNombreUsuario != null) LblNombreUsuario.Text = nombre;
        if (LblAvatar != null)
        {
            LblAvatar.Text = !string.IsNullOrEmpty(nombre)
                             ? nombre[0].ToString().ToUpper()
                             : "U";
        }
    }

    private async void CargarDatosAsync()
    {
        try
        {
            
            var combos = await _menuService.ObtenerPorCategoriaAsync("Combo");
            if (CarruselCombos != null)
            {
                if (combos != null && combos.Any())
                {
                    CarruselCombos.ItemsSource = combos;
                    CarruselCombos.IsVisible = true;
                    IndicadorCombos.IsVisible = combos.Count > 1; 
                }
                else
                {
                    CarruselCombos.IsVisible = false;
                    IndicadorCombos.IsVisible = false;
                }
            }

            // Categorías
            var hamburguesas = await _menuService.ObtenerPorCategoriaAsync("Hamburguesa");
            var alitas = await _menuService.ObtenerPorCategoriaAsync("Alita");
            var boneles = await _menuService.ObtenerPorCategoriaAsync("Boneles");
            var bebidas = await _menuService.ObtenerPorCategoriaAsync("Bebidas");

            if (LblCountHamb != null) LblCountHamb.Text = $"{hamburguesas.Count} items";
            if (LblCountAlit != null) LblCountAlit.Text = $"{alitas.Count} items";
            if (LblCountBon != null) LblCountBon.Text = $"{boneles.Count} items";
            if (LblCountBebi != null) LblCountBebi.Text = $"{bebidas.Count} items";

            // Lo más pedido
            var todos = await _menuService.ObtenerTodosAsync();
            if (ListaMasPedido != null)
                ListaMasPedido.ItemsSource = todos.Take(3).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar datos: {ex.Message}");
        }
    }

    private async void OnCategoriaClicked(object sender, TappedEventArgs e)
    {
        string categoria = e.Parameter?.ToString() ?? "Todos";
        await Navigation.PushAsync(new MenuPage(_mesa, categoria));
    }

    private async void OnVerTodoClicked(object sender, TappedEventArgs e)
    {
        var rol = SesionService.ObtenerRol();
        if(rol == "Mesero")
        {
            await Navigation.PushAsync(new PedidoDomicilioPage());
        }
        else
        {
            await Navigation.PushAsync(new MenuPage());
        }
            
    }
}