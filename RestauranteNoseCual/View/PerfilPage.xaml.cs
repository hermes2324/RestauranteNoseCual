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
    // Evento para filtrar solo números y mostrar contador
    private void OnTelefonoChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // Solo permite dígitos
        string soloNumeros = new string(entry.Text?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

        if (entry.Text != soloNumeros)
        {
            entry.Text = soloNumeros;
            return;
        }

        if (LblContadorTel != null)
        {
            int digitos = soloNumeros.Length;
            LblContadorTel.Text = $"{digitos}/10";
            LblContadorTel.TextColor = digitos == 10
                ? Color.FromArgb("#4CAF50")  // verde = completo
                : Color.FromArgb("#555555"); // gris = incompleto
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (_clienteActual == null) return;

  
        string telefono = EntryTelefono.Text?.Trim() ?? "";
        if (telefono.Length != 10 || !telefono.All(char.IsDigit))
        {
            await DisplayAlert("Atención", "El teléfono debe tener exactamente 10 dígitos.", "OK");
            return;
        }

        
        if (telefono != _clienteActual.Telefono) 
        {
            var existente = await _clienteService.BuscarPorTelefonoAsync(telefono);
            if (existente != null && existente.Id != _clienteActual.Id)
            {
                await DisplayAlert("Teléfono en uso",
                    "Este número ya está registrado por otro cliente.", "OK");
                return;
            }
        }

        
        if (string.IsNullOrWhiteSpace(EntryDomicilio.Text))
        {
            await DisplayAlert("Atención", "El domicilio no puede estar vacío.", "OK");
            return;
        }

        BtnGuardar.IsEnabled = false;
        BtnGuardar.Text = "Guardando...";

        try
        {
            _clienteActual.Nombre = EntryNombre.Text?.Trim() ?? _clienteActual.Nombre;
            _clienteActual.Telefono = telefono;
            _clienteActual.Domicilio = EntryDomicilio.Text?.Trim() ?? _clienteActual.Domicilio;
            _clienteActual.Notas = EntryNotas.Text?.Trim() ?? string.Empty;

            await _clienteService.GuardarOActualizarAsync(_clienteActual);

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