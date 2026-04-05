using RestauranteNoseCual.Services;
using RestauranteNoseCual.View;

namespace RestauranteNoseCual
{
    public partial class App : Application
    {
        public App()
        {
            // Registro de licencia de Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCe0x3WmFZfVhgdl9CYFZSQmYuP1ZhSXxVdkFjWH9cdX1UQWlbWEB9XEE=");

            InitializeComponent();

          
            if (SesionService.HaySesionActiva())
            {
                MainPage = new View.FlyoutMenuPage();
            }
            else
            {
                MainPage = new NavigationPage(new Inicio_Sesion());
            }
        }
    }
}
