using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using NTag.Models;

namespace NTag.Converters
{
    public class ResultCodeToColor : IValueConverter
    {
        private Brush _successfulColor = new SolidColorBrush(Colors.Green);
        private Brush _errorColor = new SolidColorBrush(Colors.Red);
        private Brush _defaultColor = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int resultCode)
            {
                if (resultCode == TrackModel.SuccessfulCode)
                {
                    return _successfulColor;
                }
                else if (resultCode > TrackModel.SuccessfulCode)
                {
                    return _errorColor;
                }
            }

            return _defaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
