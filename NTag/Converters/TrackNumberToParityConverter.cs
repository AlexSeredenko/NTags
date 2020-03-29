using System;
using System.Globalization;
using System.Windows.Data;

namespace NTag.Converters
{
    public class TrackNumberToParityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int trackNum)
            {
                var result = trackNum % 2 == 0;
                return result;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
