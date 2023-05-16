using Serilog.Events;
using System.ComponentModel;
using System.Globalization;

namespace SiteMapGenerator.Converters
{
    public class LogLevelConverter : TypeConverter
    {
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out LogEventLevel eventLevel))
                {
                    return eventLevel;
                }
            }
            throw new NotSupportedException("Can't convert value to LogEventLevel.");
        }
    }
}
