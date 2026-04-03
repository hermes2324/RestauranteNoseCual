using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class Pantalla_Principal : FlyoutPage
{
    private readonly AltaMenuService _menuService = new();
    Mesa _mesa;

    public Pantalla_Principal()
    {
        InitializeComponent();
        CargarUsuario();
        CargarDatosAsync();
    }

    private void CargarUsuario()
    {
        var (correo, nombre) = SesionService.ObtenerSesion();

        var hora = DateTime.Now.Hour;
        string saludo = hora < 12 ? "¡Buenos días! 🌅"
                      : hora < 18 ? "¡Buenas tardes! ☀️"
                      : "¡Buenas noches! 🌙";

        if (Detail is NavigationPage nav &&
            nav.CurrentPage is ContentPage page)
        {
            var lblSaludo = page.FindByName<Label>("LblSaludo");
            var lblNombre = page.FindByName<Label>("LblNombreUsuario");
            var lblAvatar = page.FindByName<Label>("LblAvatar");

            if (lblSaludo != null) lblSaludo.Text = saludo;
            if (lblNombre != null) lblNombre.Text = nombre;
            if (lblAvatar != null) lblAvatar.Text = nombre.Length > 0
                                                    ? nombre[0].ToString().ToUpper()
                                                    : "U";
        }
    }

    private async void CargarDatosAsync()
    {
        if (Detail is NavigationPage nav &&
            nav.CurrentPage is ContentPage page)
        {
       
            var hamburguesas = await _menuService.ObtenerPorCategoriaAsync("Hamburguesa");
            var alitas = await _menuService.ObtenerPorCategoriaAsync("Alita");
            var boneles = await _menuService.ObtenerPorCategoriaAsync("Boneles");

            var lblHamb = page.FindByName<Label>("LblCountHamb");
            var lblAlit = page.FindByName<Label>("LblCountAlit");
            var lblBon = page.FindByName<Label>("LblCountBon");

            if (lblHamb != null) lblHamb.Text = $"{hamburguesas.Count} items";
            if (lblAlit != null) lblAlit.Text = $"{alitas.Count} items";
            if (lblBon != null) lblBon.Text = $"{boneles.Count} items";

           
            var todos = await _menuService.ObtenerTodosAsync();
            var loMasPedido = page.FindByName<CollectionView>("ListaMasPedido");
            if (loMasPedido != null)
                loMasPedido.ItemsSource = todos.Take(3).ToList();
        }
    }

    private async void OnCategoriaClicked(object sender, TappedEventArgs e)
    {
        string categoria = e.Parameter?.ToString() ?? "Todos";
        if (Detail is NavigationPage nav)
        {
            await nav.PushAsync(new MenuPage(_mesa, categoria));
        }
    }

    private async void OnVerTodoClicked(object sender, TappedEventArgs e)
    {
        if (Detail is NavigationPage nav)
            await nav.PushAsync(new MenuPage());
    }
}