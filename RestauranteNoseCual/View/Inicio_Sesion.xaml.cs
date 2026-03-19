namespace RestauranteNoseCual.View;
using RestauranteNoseCual.Base_de_Datos;

public partial class Inicio_Sesion : ContentPage
{
    private readonly AutentificacionGoogle _authService = new();
    private AutentificacionGoogle.GoogleUser? _currentUser;
    public Inicio_Sesion()
	{
		InitializeComponent();
	}

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        try
        {
            _currentUser = await _authService.SignInAsync();

            if (_currentUser != null)
                await DisplayAlert("Éxito", $"Bienvenido, {_currentUser.Name}\n{_currentUser.Email}", "Continuar");
            else
                await DisplayAlert("? Cancelado", "No se completó el inicio de sesión.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("? Error", ex.Message, "OK");
        }
    }
}