using System;
using System.Globalization;
using System.Windows.Data;

namespace VolMix
{
    public class IntegerPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return $"{Math.Round(doubleValue)}%";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SliderHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double value && values[1] is double actualHeight)
            {
                double filledHeight = (value / 100) * actualHeight;
                if (parameter != null && parameter.ToString() == "Remaining")
                {
                    return actualHeight + filledHeight;
                }
                return filledHeight;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}