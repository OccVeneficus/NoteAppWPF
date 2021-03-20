using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NoteAppWpf.View.Converters
{
    /// <summary>
    /// Конвертер для даты в формат дд/мм/гггг
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime) value;
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (DateTime.TryParse(strValue, out var resultDateTime))
            {
                return resultDateTime;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
