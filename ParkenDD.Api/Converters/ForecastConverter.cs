using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkenDD.Api.Models;

namespace ParkenDD.Api.Converters
{
    public sealed class ForecastConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Load(reader);
            }
            catch
            {
                return null;
            }

            return serializer.Deserialize<ForecastBase>(jObject.CreateReader());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ForecastBase);
        }
    }
}
