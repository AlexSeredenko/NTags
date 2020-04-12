using System;
using System.Globalization;
using System.Windows.Data;
using NTag.Models;

namespace NTag.Converters
{
    public class ResultCodeToText : IValueConverter
    {
        private const string _successfulText = "Done!";
        private const string _errorText = "Error";
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int resultCode)
            {
                if (resultCode == TrackModel.SuccessfulCode)
                {
                    return _successfulText;
                }
                else if (resultCode > TrackModel.SuccessfulCode)
                {
                    return _errorText;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
