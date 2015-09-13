using System;
using Newtonsoft.Json;

namespace ParkenDD.Api.Converters
{
    public class JsonUriConverter : JsonConverter
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
            var uri = value as Uri;
            if (uri == null)
            {
                writer.WriteNull();
            } else { 
                writer.WriteValue(uri.OriginalString);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }
    }
}
