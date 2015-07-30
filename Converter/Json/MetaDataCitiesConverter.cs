using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ParkenDD.Win10.Models;

namespace ParkenDD.Win10.Converter.Json
{
    public class MetaDataCitiesConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var instance = (Dictionary<string, string>)serializer.Deserialize(reader, typeof(Dictionary<string, string>));
                var result = new MetaDataCities();
                foreach (var i in instance)
                {
                    result.Add(i.Value, new MetaDataCityRow
                    {
                        Id = i.Value,
                        Name = i.Key
                    });
                }
                return result;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is set to false.");
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MetaDataCities);
        }
    }
}
