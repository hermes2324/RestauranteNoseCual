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
            // 1. Guardamos la sesión incluyendo el Rol que viene de la base de datos
            SesionService.GuardarSesion(cliente.Id, cliente.Correo, cliente.Nombre, cliente.Rol);

            await DisplayAlert("Éxito", mensaje, "Continuar");

            // 2. Redirigimos según el rol
            RedirigirSegunRol(cliente.Rol);
        }
        else
        {
            await DisplayAlert("Error", mensaje, "OK");
        }
    }

    private async void OnLoginManualClicked(object sender, EventArgs e)
    {
        var (exito, mensaje, cliente) = await _loginController
            .LoginManualAsync(Correo.Text?.Trim(), contraseña.Text?.Trim());

        if (exito && cliente != null)
        {
           
            SesionService.GuardarSesion(cliente.Id, cliente.Correo, cliente.Nombre, cliente.Rol);

            await DisplayAlert("Éxito", mensaje, "Continuar");

            RedirigirSegunRol(cliente.Rol);
        }
        else
        {
            await DisplayAlert("Error", mensaje, "OK");
        }
    }

   
    private void RedirigirSegunRol(string rol)
    {
        if (rol == "Admin" || rol == "Mesero")
        {
           
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            Application.Current.MainPage = new AppShell();
        }
    }

    private void OnRegistrarseClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new Registro());
    }
}