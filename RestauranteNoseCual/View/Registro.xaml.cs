namespace RestauranteNoseCual.View;
using RestauranteNoseCual.Controllers;

public partial class Registro : ContentPage
{
    private readonly RegistroController _registroController = new();

    public Registro()
    {
        InitializeComponent();
    }

    private async void OnCrearCuentaClicked(object sender, EventArgs e)
    {
        var (exito, mensaje) = await _registroController.RegistrarAsync(
            Nombre.Text?.Trim(),
            Telefono.Text?.Trim(),
            Domicilio.Text?.Trim(),
            Correo.Text?.Trim(),
            Contrasena.Text?.Trim(),
            ConfirmarContrasena.Text?.Trim()
        );

        if (exito)
        {
            await DisplayAlert("✅ Éxito", mensaje, "Continuar");
            // Regresa al login después de registrarse
            Application.Current.MainPage = new NavigationPage(new Inicio_Sesion());
        }
        else
        {
            await DisplayAlert("⚠️ Aviso", mensaje, "OK");
        }
    }

    private async void OnVolverLoginClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new Inicio_Sesion());
    }
}