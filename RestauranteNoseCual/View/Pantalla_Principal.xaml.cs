using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class Pantalla_Principal : FlyoutPage
{
    public Pantalla_Principal()
    {
        InitializeComponent();
        CargarUsuario();
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

}