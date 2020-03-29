using System;
using System.Globalization;
using System.Windows.Data;

namespace NTag.Converters
{
    public class TrackNumberConverter : IValueConverter
    {
        private const string _numberPrefix = "#";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int trackNum)
            {
                return _numberPrefix + trackNum.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
