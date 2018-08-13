using System;
using System.ComponentModel;
using System.Globalization;

namespace Netsphere.Common.Converters
{
    public class TimeSpanTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
                return TimeSpan.FromMilliseconds(long.Parse(str));

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((TimeSpan)value).TotalMilliseconds.ToString("0");

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
