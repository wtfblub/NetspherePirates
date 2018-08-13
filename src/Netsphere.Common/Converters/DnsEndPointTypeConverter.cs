using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace Netsphere.Common.Converters
{
    public class DnsEndPointTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                var split = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                    throw new FormatException("Wrong format for DnsEndPoint");

                if (!ushort.TryParse(split[1], out var port))
                    throw new FormatException("Wrong format for DnsEndPoint");

                return new DnsEndPoint(split[0], port);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var dnsEndPoint = (DnsEndPoint)value;
                return $"{dnsEndPoint.Host}:{dnsEndPoint.Port}";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
