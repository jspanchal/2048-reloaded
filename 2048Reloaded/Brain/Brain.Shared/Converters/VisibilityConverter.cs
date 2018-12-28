using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#if NETFX_CORE

#endif

#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Globalization;
#endif

namespace Brain.Converters
{
    public sealed class VisibilityConverter : IValueConverter
    {
        public bool InvertResult { get; set; }

        public object Convert(object value, Type targetType, object parameter)
        {
            bool bValue = false;
            var nullableValue = value as bool?;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if ((value is Int16) || (value is Int32) || (value is Int64))
            {
                var iValue = System.Convert.ToInt64(value);
                bValue = (iValue != 0);
            }
            else if (value is string)
            {
                bValue = !string.IsNullOrWhiteSpace(value.ToString());
            }
            else
            {
                if (value != null) bValue = true;
            }

            if (parameter != null)
            {
                var bParam = System.Convert.ToBoolean(parameter);
                if (bParam)
                    bValue = !bValue;
            }

            if (InvertResult)
                bValue = !bValue;

            return bValue ? Visibility.Visible : Visibility.Collapsed;            
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
