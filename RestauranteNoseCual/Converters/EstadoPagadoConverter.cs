using System.Globalization;

namespace RestauranteNoseCual.Converters
{
    public class EstadoPagadoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si el estado es "Pagado", deshabilitar el Picker (IsEnabled = false)
            return value?.ToString() != "Pagada";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}