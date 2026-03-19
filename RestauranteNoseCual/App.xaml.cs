using RestauranteNoseCual.View;

namespace RestauranteNoseCual
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Inicio_Sesion();
        }
    }
}
