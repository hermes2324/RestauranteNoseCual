 using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;


namespace RestauranteNoseCual.View;

public partial class MenuPage : ContentPage
{
    private readonly MenuController _menuController = new();
    //private readonly Mesa _mesaSeleccionada;
    private string _categoriaActual = "Todos";
    public MenuPage() : this(null, "Todos") { }

    private readonly Mesa? _mesaSeleccionada;

    // Modifica el constructor actual
    public MenuPage(Mesa? mesa = null, string categoria = "Todos")
    {
        InitializeComponent();
        _mesaSeleccionada = mesa; 

        if (mesa == null)
        {
            Title = "Orden a Domicilio";
        }
        else
        {
            Title = $"Menú — Mesa {mesa.Numero}";
        }

        CargarProductosAsync("Todos");
    }

    //public MenuPage()
    //{
    //    InitializeComponent();
    //    CargarProductosAsync("Todos");
    //}

    //private async void CargarProductosAsync(string categoria)
    //{
    //    Cargando.IsVisible = true;
    //    Cargando.IsRunning = true;

    //    List<AltaMenu> productos;

    //    if (categoria == "Todos")
    //        productos = await _menuController.ObtenerTodosAsync();
    //    else
    //        productos = await _menuController.ObtenerPorCategoriaAsync(categoria);

    //    ListaProductos.ItemsSource = productos;

    //    Cargando.IsVisible = false;
    //    Cargando.IsRunning = false;
    //}
    private async void CargarProductosAsync(string categoria)
    {
        Cargando.IsVisible = true;
        Cargando.IsRunning = true;

        List<AltaMenu> productos;
        if (categoria == "Todos")
            productos = await _menuController.ObtenerTodosAsync();
        else
            productos = await _menuController.ObtenerPorCategoriaAsync(categoria);

        // 👇 Solo mostrar productos disponibles
        ListaProductos.ItemsSource = productos.Where(p => p.Disponible).ToList();

        Cargando.IsVisible = false;
        Cargando.IsRunning = false;
    }
    private void OnCategoriaSeleccionada(object sender, TappedEventArgs e)
    {
        string categoria = e.Parameter?.ToString() ?? "Todos";
        _categoriaActual = categoria;

        
        var tabs = new[] { BtnTodos, BtnHamburguesa, BtnAlita, BtnBonel, BtnCombos };
        foreach (var tab in tabs)
        {
            tab.BackgroundColor = Color.FromArgb("#1A1A1A");
            tab.Stroke = Color.FromArgb("#2A2A2A");
            if (tab.Content is Label lbl) lbl.TextColor = Colors.White;
        }

       
        Border seleccionado = categoria switch
        {
            "Hamburguesa" => BtnHamburguesa,
            "Alita" => BtnAlita,
            "Boneles" => BtnBonel,
            "Combo" => BtnCombos,
            "Bebidas" => BtnBebidas,
            _ => BtnTodos
        };

        seleccionado.BackgroundColor = Color.FromArgb("#F5C842");
        seleccionado.Stroke = Colors.Transparent;
        if (seleccionado.Content is Label lblSel) lblSel.TextColor = Color.FromArgb("#0D0D0D");

        CargarProductosAsync(categoria);
    }
   
    private readonly CarritoController _carritoController = new();

    private void OnAgregarAlCarrito(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var producto = (AltaMenu)button.CommandParameter;

        if (producto != null)
        {
            _carritoController.AgregarProducto(producto);


            DisplayAlert("✅ ¡Listo!", $"Se añadió '{producto.Nombre}'\nal carrito.", "OK");
        }
    }

    private async void OnVerCarrito(object sender, EventArgs e)
    {
        // Pasamos la mesa seleccionada a la página del carrito para saber a quién cobrar
        await Navigation.PushAsync(new CarritoPage(_mesaSeleccionada));
    }
}