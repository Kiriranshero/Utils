using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Script.Serialization;

namespace ServerFramework
{
    public interface ISerializer
    {
        IDictionary<string,object> Deserialize(IDictionary items, Stream inputStream, Encoding encoding);
        void Serialize(IDictionary items, object result, Stream outputStream, Encoding encoding);
    }

    class JsonSerializer : ISerializer
    {
        internal static UnixDateConverter UnixDate = new UnixDateConverter();

        internal static JsonSerializerSettings Setting = new JsonSerializerSettings 
        {
            Converters = new List<JsonConverter> { new DataRowConverter(), new CustomExpandoObjectConverter() } 
        };

        public IDictionary<string, object> Deserialize(IDictionary items, Stream inputStream, Encoding encoding)
        {
            using (var Reader = new StreamReader(inputStream, encoding))
                return JsonConvert.DeserializeObject<ExpandoObject>(Reader.ReadToEnd(), Setting);          
        }

        public void Serialize(IDictionary items, object result, Stream outputStream, Encoding encoding)
        {
            using (var Writer = new StreamWriter(outputStream, encoding))
                Writer.Write(JsonConvert.SerializeObject(result, Setting));
        }
    }

}
