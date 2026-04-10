using Microsoft.Maui.Controls;

namespace RestauranteNoseCual.View;

public class MainFlyoutPage : FlyoutPage
{
    public MainFlyoutPage()
    {
        // Menú lateral
        Flyout = new FlyoutMenuPage();

        // Pantalla principal (la primera que se ve)
        Detail = new NavigationPage(new Pantalla_Principal());
    }
}