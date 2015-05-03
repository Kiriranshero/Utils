using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Data;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Xml.XPath;

namespace ServerFramework
{
    public static class Extensions
    {
        internal static XmlCommentDocumentationProvider RestExplorer = new XmlCommentDocumentationProvider(ServerConfig.HelpPath);

        private static string Value(this XPathNavigator inThis)
        {
            if (inThis == null) return "No Documentation Found.";
            return inThis.Value.Trim();
        }

        public static string Help(this Type inThis)
        {
            return RestExplorer.GetClass(inThis).Value();
        }

        public static List<string> Help(this Type inThis, string[] Keys)
        {
            var Result = new List<string>();
            foreach (var Key in Keys)
            {
                var tValue = RestExplorer.GetClass(inThis, Key);
                if (tValue == null) continue;
                Result.Add(string.Format("{0}: {1}", Key, tValue.Value()));
            }
            return Result;
        }

        public static string Help(this MethodInfo inThis)
        {
            return RestExplorer.GetMethod(inThis).Value();
        }
        public static List<string> Help(this MethodInfo inThis, string[] Keys)
        {
            var Result = new List<string>();
            foreach (var Key in Keys)
            {
                var tValue = RestExplorer.GetMethod(inThis, Key);
                if (tValue == null) continue;
                Result.Add(string.Format("{0}: {1}", Key, tValue.Value()));
            }
            return Result;
        }

        public static string Help(this PropertyInfo inThis)
        {
            return RestExplorer.GetProperty(inThis).Value();
        }

        public static string EnumHelp(this Type inThis, object enumValue)
        {
            return RestExplorer.GetEnum(inThis, Enum.ToObject(inThis, enumValue)).Value();
        }

        
        public static string GenericName(this Type inThis)
        {            
            var tResult = new StringBuilder();
            if (inThis.IsGenericType)
            {
                tResult.Append(inThis.Name.Substring(0, inThis.Name.IndexOf('`')));
                tResult.Append("&lt");
                foreach (var Arg in inThis.GenericTypeArguments)
                    tResult.Append(Arg.Name);
                tResult.Append(">");
            }
            else
                tResult.Append(inThis.Name);
            return tResult.ToString();
        }



        public static Random RN = new Random();

        private const string CType = "type";
        private const string CName = "name";
        private const string CProperties = "properties";

        private const string CCtrl = "Controller";

        private const string CClass = "class";
        private const string CEnum = "enum";
        private const string CStruct = "struct";
        private const string CValue = "value";
        private const string CArray = "array";

        internal static bool IsSystem(this Type inType)
        {
            return inType.Namespace == "System";
        }

        public static string CtrlName(this Type inThis, string remove = "Controller")
        {
            return Regex.Replace(inThis.Name, remove, "", RegexOptions.IgnoreCase);
        }

        internal static XmlCommentDocumentationProvider GetRestExplorer(this ServicesContainer services)
        {
            return (XmlCommentDocumentationProvider)services.GetDocumentationProvider();
        }

        internal static void AppendData(this StringBuilder inBuilder, string inFormat, params object[] inDatas)
        {
            var inRed = false;
            foreach (var inData in inDatas)
            {
                if (inData.ToString().Contains("!") == false) continue;
                inRed = true;
                break;
            }

            if (inRed)
                inBuilder.AppendFormat(string.Format("<font color=red>{0}</font>", inFormat), inDatas);
            else
                inBuilder.AppendFormat(inFormat, inDatas);
        }

        
        internal static string Ctrl(this RouteData inthis)
        {
            return inthis.Values["controller"] as string;
        }
        internal static string Action(this RouteData inthis)
        {
            return inthis.Values["action"] as string;
        }

        internal static bool IsJson(this ControllerContext inThis)
        {
            return inThis.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        }
        public static DateTime toTime(this short inThis)
        {
            return UnixDateConverter.EPOC.AddSeconds(inThis);
        }

        public static DateTime toTime(this int inThis)
        {
            return UnixDateConverter.EPOC.AddSeconds(inThis);
        }
    }
}   
