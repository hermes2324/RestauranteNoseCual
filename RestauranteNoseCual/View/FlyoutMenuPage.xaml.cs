using Microsoft.Maui.Controls;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;
using Syncfusion.Maui.DataGrid;

namespace RestauranteNoseCual.View;

public partial class FlyoutMenuPage : FlyoutPage  
{
    Controllers.Control_Cliente _controlCliente = new();
    public FlyoutMenuPage()
    {
        InitializeComponent();
        CargarMenuPorRol();
    }

    private async void CargarMenuPorRol()
    {
        var sesion = SesionService.ObtenerSesion();
        string rol = await _controlCliente.ObtenerRolRealAsync(sesion.correo);
        var menuItems = new List<FlyoutPageItem>
        {
            new() { Title = "Inicio", IconSource = "🏠", TargetType = typeof(Pantalla_Principal) }
        };

        if (rol == "Admin" || rol == "Mesero")
        {
            menuItems.Add(new() { Title = "Pedidos", IconSource = "📦", TargetType = typeof(Pedidos) });
            menuItems.Add(new() { Title = "Crear Orden", IconSource = "🪑", TargetType = typeof(SeleccionMesaPage) });
        }
        if (rol == "Cliente")
        {
            menuItems.Add(new() { Title = "Mi Perfil", IconSource = "👤", TargetType = typeof(PerfilPage) });
            menuItems.Add(new() { Title = "Hacer Pedido", IconSource = "🍕", TargetType = typeof(MenuPage) });     
            menuItems.Add(new() { Title = "Mis Órdenes", IconSource = "📜", TargetType = typeof(MisPedidosPage) });
        }
        menuItems.Add(new() { Title = "Cerrar Sesión", IconSource = "🚪", TargetType = null });

        menuItemsCollection.ItemsSource = menuItems;
    }

    private async void OnMenuItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not FlyoutPageItem item)
            return;

        IsPresented = false; // ← directo, ya somos FlyoutPage

        if (item.TargetType == null)
        {
            bool confirm = await DisplayAlert("Cerrar sesión", "¿Seguro que deseas salir?", "Sí", "No");
            if (confirm)
            {
                SesionService.CerrarSesion();
                Application.Current.MainPage = new NavigationPage(new Inicio_Sesion());
            }
            return;
        }

        var page = (Page)Activator.CreateInstance(item.TargetType);
        Detail = new NavigationPage(page) { BackgroundColor = Color.FromArgb("#0D0D0D") };
        menuItemsCollection.SelectedItem = null;
    }
}