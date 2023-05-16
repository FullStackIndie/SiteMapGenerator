using SiteMapGenerator.Data.Enums;
using System.ComponentModel;
using System.Globalization;

namespace SiteMapGenerator.Converters
{
    internal class ChangeFrequencyTypeConverter : TypeConverter
    {

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string strValue)
            {
                if (Enum.TryParse(strValue, out ChangeFrequency changeFrequency))
                {
                    return changeFrequency;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
