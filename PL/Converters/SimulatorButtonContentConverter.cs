using System.Windows.Data;

namespace PL
{
    public class SimulatorButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isRunning)
            {
                return isRunning ? "Stop Simulator" : "Start Simulator";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}