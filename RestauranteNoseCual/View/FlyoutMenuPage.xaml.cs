using Microsoft.Maui.Controls;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class FlyoutMenuPage : ContentPage
{
    public FlyoutMenuPage()
    {
        InitializeComponent();

        menuItemsCollection.ItemsSource = new List<FlyoutPageItem>
        {
            new FlyoutPageItem { Title = "Inicio",        IconSource = "🏠", TargetType = typeof(Pantalla_Principal) },
            //new FlyoutPageItem { Title = "Ver Menú", IconSource = "🍽️", TargetType = typeof(MenuPage) },
            new FlyoutPageItem { Title = "Pedidos",   IconSource = "📦", TargetType = typeof(Pedidos) },
            new FlyoutPageItem { Title = "Crear Orden", IconSource = "🪑", TargetType = typeof(SeleccionMesaPage) },
            new FlyoutPageItem { Title = "Cerrar Sesión", IconSource = "🚪", TargetType = null },
        };
    }

    private async void OnMenuItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not FlyoutPageItem item)
            return;

        if (Application.Current.MainPage is FlyoutPage flyoutPage)
            flyoutPage.IsPresented = false;

        if (item.TargetType == null)
        {
            bool confirm = await DisplayAlert("Cerrar sesión",
                                              "¿Seguro que deseas salir?",
                                              "Sí", "No");
            if (confirm)
            {
                
                SesionService.CerrarSesion();
                Application.Current.MainPage = new NavigationPage(new Inicio_Sesion());
            }
            return;
        }

        var page = (Page)Activator.CreateInstance(item.TargetType);

        if (Application.Current.MainPage is FlyoutPage fp &&
            fp.Detail is NavigationPage navPage)
        {
            await navPage.PushAsync(page);
        }
    }
}