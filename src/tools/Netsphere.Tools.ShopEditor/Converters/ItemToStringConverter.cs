using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Netsphere.Tools.ShopEditor.Services;

namespace Netsphere.Tools.ShopEditor.Converters
{
    public class ItemToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var item = (Item)value;
            return $"{item.ItemNumber.Id} - {item.Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
