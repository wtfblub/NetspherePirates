using System;
using System.ComponentModel;
using System.Globalization;

namespace Netsphere.Common.Converters
{
    public class ItemNumberTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
                return new ItemNumber(uint.Parse(str));

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(ItemNumber))
                return ((ItemNumber)value).Id;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
