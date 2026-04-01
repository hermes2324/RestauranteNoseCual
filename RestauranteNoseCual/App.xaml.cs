using RestauranteNoseCual.Services;
using RestauranteNoseCual.View;

namespace RestauranteNoseCual
{
    public partial class App : Application
    {
        public App()
        {
            // 1. Registro de la licencia de Syncfusion para la versión 28.2.12
            // Esto debe ir SIEMPRE antes de InitializeComponent()
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCe0x3WmFZfVhgdl9CYFZSQmYuP1ZhSXxVdkFjWH9cdX1UQWlbWEB9XEE=");

            InitializeComponent();

            // 2. Lógica de navegación basada en la sesión activa
            if (SesionService.HaySesionActiva())
            {
                // Si ya inició sesión, va directo al dashboard
                MainPage = new Pantalla_Principal();
            }
            else
            {
                // Si no, lo manda al Login envuelto en un NavigationPage para poder navegar
                MainPage = new NavigationPage(new Inicio_Sesion());
            }
        }
    }
}
