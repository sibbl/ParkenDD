using System;
using Newtonsoft.Json;

namespace ParkenDD.Api.Converters
{
    public class JsonUtcDateTimeConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            var utcTime = DateTime.Parse(reader.Value.ToString());
            return TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, TimeZoneInfo.Local);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(TimeZoneInfo.ConvertTime((DateTime)value, TimeZoneInfo.Utc));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
