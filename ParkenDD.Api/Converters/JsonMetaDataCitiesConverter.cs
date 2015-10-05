using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ParkenDD.Api.Models;

namespace ParkenDD.Api.Converters
{
    public class JsonMetaDataCitiesConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var dict = (Dictionary<string, MetaDataCityRow>)serializer.Deserialize(reader, typeof(Dictionary<string, MetaDataCityRow>));
                var result = new MetaDataCities();
                foreach (var i in dict)
                {
                    i.Value.Id = i.Key;
                    result.Add(i.Value);
                }
                return result;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var cities = value as MetaDataCities;
            if (cities == null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, cities.ToDictionary(x => x.Name, x => x.Id));
            }
        }

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MetaDataCities);
        }
    }
}
