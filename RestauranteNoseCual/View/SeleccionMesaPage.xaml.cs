using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Controllers;
using RestauranteNoseCual.Models;
using Supabase.Realtime.PostgresChanges;
using System.Collections.ObjectModel;

namespace RestauranteNoseCual.View;

public partial class SeleccionMesaPage : ContentPage
{
    private readonly MesaController _mesaController = new();
    private ObservableCollection<Mesa> _mesas = new();
    private Supabase.Realtime.RealtimeChannel _channel;

    public SeleccionMesaPage()
    {
        InitializeComponent();
        CargarMesasAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IniciarRealtimeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_channel != null)
        {
            _channel.Unsubscribe();
            _channel = null;
        }
    }

    private async void CargarMesasAsync()
    {
        var mesas = await _mesaController.ObtenerMesasAsync();
        _mesas = new ObservableCollection<Mesa>(mesas);
        ListaMesas.ItemsSource = _mesas;
        Cargando.IsVisible = false;
        Cargando.IsRunning = false;
        ListaMesas.IsVisible = true;
    }
    private async Task IniciarRealtimeAsync()
    {
        try
        {
            // Habilitar Realtime en tabla Mesa primero en Supabase Dashboard
            _channel = Conexion.Supabase.Realtime.Channel("realtime:public:Mesa");

            var postgresChanges = _channel.Register(
                new PostgresChangesOptions("public", "Mesa")
            );

            postgresChanges.AddPostgresChangeHandler(
                PostgresChangesOptions.ListenType.Updates,
                (_, change) =>
                {
                    var mesaActualizada = change.Model<Mesa>();
                    if (mesaActualizada == null) return;

                    Console.WriteLine($"[REALTIME] Mesa {mesaActualizada.Numero} ? {mesaActualizada.Estado}");
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var mesaExistente = _mesas.FirstOrDefault(m => m.Id == mesaActualizada.Id);
                        if (mesaExistente != null)
                        {
                            mesaExistente.Estado = mesaActualizada.Estado; // ?? Esto solo ya actualiza la UI
                        }
                    });
                }
            );

            await _channel.Subscribe();
            Console.WriteLine("[REALTIME] SeleccionMesa escuchando cambios...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[REALTIME] Error en SeleccionMesa: {ex.Message}");
        }
    }

    private async void OnMesaSeleccionada(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Mesa mesa) return;

        if (mesa.Estado == "Ocupada")
        {
            await DisplayAlert("Mesa ocupada",
                               $"La Mesa {mesa.Numero} ya está ocupada",
                               "OK");
            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar",
            $"¿Iniciar orden para la Mesa {mesa.Numero}?",
            "Sí", "No");
        if (!confirmar) return;

        await _mesaController.OcuparMesaAsync(mesa.Id);
        await Navigation.PushAsync(new MenuPage(mesa));
    }

    private async void OnPedidoDomicilioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PedidoDomicilioPage());
    }
}