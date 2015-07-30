using System;
using Newtonsoft.Json;

namespace ParkenDD.Win10.Converter.Json
{
    public class UriConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                return null;
            }

            var value = reader.Value.ToString();

            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                return new Uri(value);
            }
            if (value.StartsWith("//") && Uri.IsWellFormedUriString("http:" + value, UriKind.Absolute))
            {
                return new Uri("http:" + value);
            }
            if (value.StartsWith("www.") && Uri.IsWellFormedUriString("http://" + value, UriKind.Absolute))
            {
                return new Uri("http://" + value);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is Uri)
            {
                writer.WriteValue(((Uri)value).OriginalString);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }
    }
}
