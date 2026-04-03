// Converters/EstadoColorConverter.cs
using System.Globalization;

namespace RestauranteNoseCual.Converters
{
    public class EstadoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Pagada" => Color.FromArgb("#E53935"), 
                "Pendiente" => Color.FromArgb("#43A047"), 
                "En proceso" => Color.FromArgb("#FB8C00"), 
                _ => Colors.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}