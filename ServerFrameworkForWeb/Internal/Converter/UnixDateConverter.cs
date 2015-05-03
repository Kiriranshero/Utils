using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.Script.Serialization;

namespace ServerFramework
{
    class UnixDateConverter : DateTimeConverterBase
    {
        /// <summary>
        /// 1970.01.01 UTC
        /// </summary>
        public static DateTime EPOC = new DateTime(0x089f7ff5f7b58000, DateTimeKind.Utc);

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {            
            long TotalSecond;
            long.TryParse(reader.Value.ToString(), out TotalSecond);
            return EPOC.AddSeconds(TotalSecond);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {            
            var ValueTime = (DateTime)value;
            if (ValueTime.Kind != DateTimeKind.Utc) ValueTime = ValueTime.ToUniversalTime();
            writer.WriteValue((long)(ValueTime - EPOC).TotalSeconds);
        }
    }

   
}
