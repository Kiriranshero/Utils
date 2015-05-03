using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ServerFramework
{
    class ObjectResult : ActionResult
    {
        public object Content { get; set; }
        internal bool Cached { get; set; }

        internal static ServerConfigBase FinalizerBase = null;

        internal static Dictionary<Type, MethodInfo> Finalizer = new Dictionary<Type, MethodInfo>(comparer: null);

        public override void ExecuteResult(ControllerContext context)
        {
            if (Cached)
                context.HttpContext.Response.Write(Content);
            else
            {
                var Type = Content.GetType();
                if (Finalizer.ContainsKey(Type)) Finalizer[Type].Invoke(FinalizerBase, new object[] { context.HttpContext, Content });

                if (CustomProviderFactory.Crypto == null)
                    CustomProviderFactory.Serializer.Serialize(context.HttpContext.Items, Content, context.HttpContext.Response.OutputStream, context.HttpContext.Response.ContentEncoding);
                else
                    using (var TempStream = new CustomStream())
                    {
                        CustomProviderFactory.Serializer.Serialize(context.HttpContext.Items, Content, TempStream, context.HttpContext.Response.ContentEncoding);
                        TempStream.Position = 0;
                        CustomProviderFactory.Crypto.Encrypt(context.HttpContext.Items, TempStream, context.HttpContext.Response.OutputStream);                        
                    }
                if (RestAuthAttribute.Cache != null) RestAuthAttribute.Cache.SetCache(context.HttpContext.Request.Headers, context.HttpContext.Response.OutputStream);
            }     
        }
    }
}
