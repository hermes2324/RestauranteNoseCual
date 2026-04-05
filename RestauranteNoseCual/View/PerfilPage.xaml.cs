using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.View;

public partial class PerfilPage : ContentPage
{
    private readonly ClienteService _clienteService = new();
    private Cliente _clienteActual;

    public PerfilPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarPerfilAsync();
    }

    private async Task CargarPerfilAsync()
    {
        try
        {
            var sesion = SesionService.ObtenerSesion();
            _clienteActual = await _clienteService.ObtenerPorCorreoAsync(sesion.correo);

            if (_clienteActual == null) return;

            // Header
            LblAvatar.Text = !string.IsNullOrEmpty(_clienteActual.Nombre)
                             ? _clienteActual.Nombre[0].ToString().ToUpper()
                             : "U";
            LblNombre.Text = _clienteActual.Nombre;
            LblRol.Text = _clienteActual.Rol;

            // Campos
            EntryNombre.Text = _clienteActual.Nombre;
            EntryCorreo.Text = _clienteActual.Correo;
            EntryTelefono.Text = _clienteActual.Telefono;
            EntryDomicilio.Text = _clienteActual.Domicilio;
            EntryNotas.Text = _clienteActual.Notas;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando perfil: {ex.Message}");
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (_clienteActual == null) return;

        BtnGuardar.IsEnabled = false;
        BtnGuardar.Text = "Guardando...";

        try
        {
            _clienteActual.Nombre = EntryNombre.Text?.Trim() ?? _clienteActual.Nombre;
            _clienteActual.Telefono = EntryTelefono.Text?.Trim() ?? _clienteActual.Telefono;
            _clienteActual.Domicilio = EntryDomicilio.Text?.Trim() ?? _clienteActual.Domicilio;
            _clienteActual.Notas = EntryNotas.Text?.Trim() ?? string.Empty;

            await _clienteService.GuardarOActualizarAsync(_clienteActual);

            // Actualiza nombre en sesión
            SesionService.GuardarSesion(
                _clienteActual.Id,
                _clienteActual.Correo,
                _clienteActual.Nombre,
                _clienteActual.Rol
            );

            LblNombre.Text = _clienteActual.Nombre;
            LblAvatar.Text = _clienteActual.Nombre[0].ToString().ToUpper();

            await DisplayAlert("✅ Listo", "Perfil actualizado correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
        }
        finally
        {
            BtnGuardar.IsEnabled = true;
            BtnGuardar.Text = "💾 Guardar Cambios";
        }
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cerrar sesión", "¿Seguro que deseas salir?", "Sí", "No");
        if (confirm)
        {
            SesionService.CerrarSesion();
            Application.Current.MainPage = new NavigationPage(new Inicio_Sesion());
        }
    }
}