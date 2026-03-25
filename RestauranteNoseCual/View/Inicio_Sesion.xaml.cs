using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;
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

        if (exito)
        {
            await DisplayAlert("Éxito", mensaje, "Continuar");
            Application.Current.MainPage = new Pantalla_Principal();
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

        if (exito)
        {
            await DisplayAlert("Éxito", mensaje, "Continuar");
            Application.Current.MainPage = new Pantalla_Principal();
        }
        else
        {
            await DisplayAlert("Error", mensaje, "OK");
        }
    }

    private void OnRegistrarseClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new Registro());
    }
}