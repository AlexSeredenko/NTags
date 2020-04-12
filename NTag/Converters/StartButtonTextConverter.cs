using System;
using System.Globalization;
using System.Windows.Data;

namespace NTag.Converters
{
    public class StartButtonTextConverter : IValueConverter
    {
        private const string _startText = "Start";
        private const string _stopText = "Stop";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool resultCode)
            {
                if (resultCode == true)
                {
                    return _stopText;
                }
                else
                {
                    return _startText;
                }
            }

            throw new ApplicationException($"Wrong input value for {nameof(StartButtonTextConverter)}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
