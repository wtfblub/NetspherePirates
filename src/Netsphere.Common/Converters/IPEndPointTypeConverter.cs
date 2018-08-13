using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace Netsphere.Common.Converters
{
    public class IPEndPointTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                var arr = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                return new IPEndPoint(IPAddress.Parse(arr[0]), int.Parse(arr[1]));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((IPEndPoint)value).ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
