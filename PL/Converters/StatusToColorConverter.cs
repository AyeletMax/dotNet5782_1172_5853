using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PL.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = value as string;
            return status switch
            {
                "Open" => Brushes.LightGreen,
                "InProgress" => Brushes.LightYellow,
                "Closed" => Brushes.LightGray,
                _ => Brushes.White
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
