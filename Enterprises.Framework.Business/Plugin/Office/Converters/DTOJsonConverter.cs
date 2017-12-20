using System;
using Enterprises.Framework.Plugin.Office.Converters.Model;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    public class DataSourceJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataSource).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            DataSource dataSource = existingValue as DataSource ?? new DataSource();

            JObject obj = JObject.Load(reader);
            serializer.Converters.Add(new DataItemJsonConverter());
            foreach (JProperty item in obj.Children())
            {
                dataSource.Add(item.Name, item.Value.ToObject<DataItem>(serializer));
            }
            return dataSource;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var dataSource = value as DataSource;
            if (dataSource != null)
            {
                serializer.Converters.Add(new DataItemJsonConverter());
                writer.WriteStartObject();
                foreach (var item in dataSource)
                {
                    writer.WritePropertyName(item.Key);
                    serializer.Serialize(writer, item.Value);
                }
                writer.WriteEndObject();
            }
        }
    }

    public class DataItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataItem).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var data = existingValue as DataItem ?? new DataItem();

            serializer.Converters.Add(new DataTableConverter());
            JObject obj = JObject.Load(reader);
            data.Type = (RenderMode) obj.Value<JToken>("Type").ToObject<int>();
            if (data.Type == RenderMode.List)
            {
                data.Value = obj.Value<JToken>("Value").ToObject<DataTable>(serializer);
            }
            else
            {
                data.Value = obj.Value<object>("Value");
            }
            return data;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var data = value as DataItem;
            if (data != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Type");
                writer.WriteValue(data.Type);
                writer.WritePropertyName("Value");
                serializer.Converters.Add(new DataTableConverter());
                serializer.Serialize(writer, data.Value);
                writer.WriteEndObject();
            }
        }
    }

}
