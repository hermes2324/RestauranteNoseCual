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
                "En proceso de entrega" => Color.FromArgb("#3B82F6"), 
                "Entregado" => Color.FromArgb("#10B981"), 
                "En preparación" => Color.FromArgb("#F59E0B"), 
                "Cancelado por el cliente" => Color.FromArgb("#EF4444"),
                "Pagada" => Color.FromArgb("#8B5CF6"),
                _ => Colors.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}