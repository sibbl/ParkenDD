using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ParkenDD.Api.Models;

namespace ParkenDD.Api.Converters
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
            var cities = value as MetaDataCities;
            if (cities == null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, cities.Select(x => x.Value).ToDictionary(x => x.Name, x => x.Id));
            }
        }

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MetaDataCities);
        }
    }
}
