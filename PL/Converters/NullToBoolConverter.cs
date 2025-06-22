using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace PL.Converters
//{
//    internal class NullToBoolConverter
//    {
//    }
//}
//using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Call
{
    public class NullToBoolConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Invert ? value == null : value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
