using System;
using FlatFile.Core;

namespace Escher.Data.Engine.Flat
{
    public class ShortDateFormat : ITypeConverter
    {
        public bool CanConvertFrom(Type type)
        {
            return type == typeof(string);
        }

        public bool CanConvertTo(Type type)
        {
            return type == typeof(DateTime);
        }

        public string ConvertToString(object source)
        {
            var date = (DateTime) source;
            return date.ToShortDateString();
        }

        public object ConvertFromString(string source)
        {
            return DateTime.Parse(source);
        }
    }
}