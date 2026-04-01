using RestauranteNoseCual.Services;
using RestauranteNoseCual.View;

namespace RestauranteNoseCual
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            if (SesionService.HaySesionActiva())
                MainPage = new Pantalla_Principal();
            else
                MainPage = new NavigationPage(new Inicio_Sesion());
        }
    }
}
