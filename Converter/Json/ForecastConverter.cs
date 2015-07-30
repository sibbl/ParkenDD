using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkenDD.Win10.Models;

namespace ParkenDD.Win10.Converter.Json
{
    public class ForecastConverter : JsonConverter
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
            throw new NotImplementedException("Unnecessary because CanWrite is set to false.");
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ForecastBase);
        }
    }
}
