using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Web.Script.Serialization;


namespace ServerFramework
{        
    class CustomProviderFactory : ValueProviderFactory
    {
        internal static ISerializer _Serializer = null;
        internal static ISerializer JsonSerializer = new JsonSerializer();
        internal static ICrypto Crypto = null;

        internal static IgnoreWorker Ignore = new IgnoreWorker();

        public static ISerializer Serializer
        {
            get { return _Serializer ?? JsonSerializer; }
            set { _Serializer = value; }
        }

        public override IValueProvider GetValueProvider(ControllerContext context)
        {
            Stream InputStream = context.HttpContext.Request.InputStream;
            
            var tSerializer = JsonSerializer;
            if (context.Controller is RestController)
            {
                tSerializer = Serializer;
                if ((Crypto != null) && (Ignore.IsDefined(context.RouteData) == false))
                    InputStream = Crypto.Decrypt(context.HttpContext.Request.Headers, context.HttpContext.Items, InputStream);
            }
            else if (context.IsJson() == false) return null;

            var ResultObject = tSerializer.Deserialize(context.HttpContext.Items, InputStream, context.HttpContext.Request.ContentEncoding);
            if (ResultObject == null) return null;
            return new DictionaryValueProvider<object>(ResultObject, CultureInfo.CurrentCulture);
        }       
    }
}
