using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    class DataRowConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(DataRow));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var Row = value as DataRow;
            writer.WriteStartObject();
            foreach (DataColumn Column in Row.Table.Columns)
            {
                writer.WritePropertyName(Column.ColumnName);
                serializer.Serialize(writer, Row[Column]);
            }
            writer.WriteEnd();
        }
    }
}
