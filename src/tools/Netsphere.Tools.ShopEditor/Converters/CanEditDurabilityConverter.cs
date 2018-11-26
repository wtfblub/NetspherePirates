using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Netsphere.Tools.ShopEditor.Converters
{
    public class CanEditDurabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var periodType = (ItemPeriodType)value;
            switch (periodType)
            {
                case ItemPeriodType.None:
                    return true;

                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
