using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;
namespace RestauranteNoseCual.View;

public partial class Inicio_Sesion : ContentPage
{
    private readonly LoginController _loginController = new();
    public Inicio_Sesion()
	{
		InitializeComponent();
	}
    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        var (exito, mensaje, cliente) = await _loginController.LoginGoogleAsync();

        if (exito && cliente != null)
        {
            SesionService.GuardarSesion(cliente.Id, cliente.Correo, cliente.Nombre, cliente.Rol);
            await DisplayAlert("Éxito", mensaje, "Continuar");

           
            bool perfilIncompleto = string.IsNullOrWhiteSpace(cliente.Telefono)
                                 || string.IsNullOrWhiteSpace(cliente.Domicilio);

            if (perfilIncompleto && cliente.Rol == "Cliente")
            {
                await DisplayAlert(
                    "Completa tu perfil",
                    "Para hacer pedidos necesitamos tu teléfono y domicilio.",
                    "Entendido");


                var flyout = new View.FlyoutMenuPage();
                Application.Current.MainPage = flyout;

                await flyout.Detail.Navigation.PushAsync(new View.PerfilPage());
            }
            else
            {
                RedirigirSegunRol(cliente.Rol);
            }
            //if (perfilIncompleto && cliente.Rol == "Cliente")
            //{
            //    await DisplayAlert(
            //        "Completa tu perfil",
            //        "Para hacer pedidos necesitamos tu teléfono y domicilio.",
            //        "Entendido");

            //    // ✅ Usar NavigationPage directamente para poder hacer Push
            //    Application.Current.MainPage = new NavigationPage(new View.PerfilPage());
            //}
            //else
            //{
            //    RedirigirSegunRol(cliente.Rol);
            //}
        }
        else
        {
            await DisplayAlert("Error", mensaje, "OK");
        }
    }

    //private async void OnLoginManualClicked(object sender, EventArgs e)
    //{
    //    var (exito, mensaje, cliente) = await _loginController
    //        .LoginManualAsync(Correo.Text?.Trim(), contraseña.Text?.Trim());

    //    if (exito && cliente != null)
    //    {

    //        SesionService.GuardarSesion(cliente.Id, cliente.Correo, cliente.Nombre, cliente.Rol);

    //        await DisplayAlert("Éxito", mensaje, "Continuar");

    //        RedirigirSegunRol(cliente.Rol);
    //    }
    //    else
    //    {
    //        await DisplayAlert("Error", mensaje, "OK");
    //    }
    //}
    private async void OnLoginManualClicked(object sender, EventArgs e)
    {
        var (exito, mensaje, cliente) = await _loginController
            .LoginManualAsync(Correo.Text?.Trim(), contraseña.Text?.Trim());

        if (exito && cliente != null)
        {
            SesionService.GuardarSesion(cliente.Id, cliente.Correo, cliente.Nombre, cliente.Rol);
            await DisplayAlert("Éxito", mensaje, "Continuar");

            // ✅ Misma lógica que Google Login
            bool perfilIncompleto = string.IsNullOrWhiteSpace(cliente.Telefono)
                                 || string.IsNullOrWhiteSpace(cliente.Domicilio);

            if (perfilIncompleto && cliente.Rol == "Cliente")
            {
                await DisplayAlert(
                    "Completa tu perfil",
                    "Para hacer pedidos necesitamos tu teléfono y domicilio.",
                    "Entendido");

                Application.Current.MainPage = new NavigationPage(new View.PerfilPage());
            }
            else
            {
                RedirigirSegunRol(cliente.Rol);
            }
        }
        else
        {
            await DisplayAlert("Error", mensaje, "OK");
        }
    }


    private void RedirigirSegunRol(string rol)
    {
        Application.Current.MainPage = new View.FlyoutMenuPage();
    }

    private void OnRegistrarseClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new Registro());
    }
}