using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Description;
using System.Security.Policy;
using System.Diagnostics;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Web.UI;
using Newtonsoft.Json;
using System.Net;

namespace ServerFramework
{
    using Tag = HtmlTextWriterTag;
    using At = HtmlTextWriterAttribute;

    /// <summary>
    /// Default
    /// </summary>
    public partial class HelpController : Controller
    {
        public HelpController()
        {
            if (ServerConfig.EnableHelp == false) throw new HttpException(404, "Page not found");
        }

        public void Index()
        {
            var Writer = new TagWriter(Response);
            using (Writer.BeginTag(Tag.Noscript))
            {
                Writer.Write("이 사이트의 기능을 모두 활용하기 위해서는 자바스크립트를 활성화 시킬 필요가 있습니다.");
                using (Writer.Add(At.Href, "http://www.enable-javascript.com/ko").Add(At.Target, "_blank").BeginTag(Tag.A))
                    Writer.Write("브라우저에서 자바스크립트를 활성화하는 방법");
                Writer.Write("을 참고 하세요.");
            }
            using(Writer.Add(At.Cols, "5%, 5%, *").BeginTag(Tag.Frameset))                
            {
                Writer.Add(At.Src, URI() + "/HelpList").Tag(Tag.Frame);
                Writer.Add(At.Src, URI() + "/TestList").Tag(Tag.Frame);
                Writer.Add(At.Name, Define.View).Tag(Tag.Frame);
            }
        }

        private string URI()
        {
            return string.Format("{0}/{1}", Request.ApplicationPath, this.GetType().CtrlName()).Replace("//", "/");
        }
    }

    /// <summary>
    /// API List
    /// </summary>
    public partial class HelpController
    {
        public static string _APIList = null;
        internal static IHelp CustomHelp = null;

        public string API()
        {
            if (_APIList != null) return _APIList;

            var CustomList = CustomHelp == null ? new string[] { } : CustomHelp.GetNode();

            var InnerTypeList = new HashSet<Type>();

            var Writer = new TagWriter();
            Writer.Tag(Tag.H1, "API");

            foreach (var Ctrl in ServerConfig.RestList)
            {
                Writer.Tag(Tag.H2, Ctrl.Key.Help());

                foreach (var Value in Ctrl.Key.Help(CustomList))
                    Writer.Tag(HtmlTextWriterTag.H3, Value);

                var ApiIndex = 0;
                foreach (var MethodInfo in Ctrl.Value)
                {
                    Writer.Tag(Tag.H3, ++ApiIndex, ". ", MethodInfo.Help());

                    foreach (var Value in MethodInfo.Help(CustomList))
                        Writer.Tag(Tag.H4, Value);

                    using (Writer.BeginTag(Tag.Ul))
                    {
                        Writer.Tag(Tag.Li, "URI: /", Ctrl.Key.CtrlName(), "/", MethodInfo.Name);

                        #region Request
                        var ParamList = MethodInfo.GetParameters();
                        if (ParamList.Count() > 0)
                            using (Writer.BeginTag(Tag.Li, Define.Request))
                            using (Writer.BeginTable(true, Define.Headers))
                                foreach (var PropertyInfo in ParamList[0].ParameterType.GetProperties())
                                    if (PropertyInfo.GetCustomAttribute(typeof(ScriptIgnoreAttribute)) != null) continue;
                                    else Writer.TagTR(InnerTypeList, PropertyInfo);
                        #endregion

                        #region Response
                        if (MethodInfo.ReturnType.IsSystem() == false)
                            using (Writer.BeginTag(Tag.Li, Define.Response))
                            using (Writer.BeginTable(true, Define.Headers))
                                foreach (var PropertyInfo in MethodInfo.ReturnType.GetProperties())
                                    if (PropertyInfo.GetCustomAttribute(typeof(ScriptIgnoreAttribute)) != null) continue;
                                    else Writer.TagTR(InnerTypeList, PropertyInfo);
                        #endregion
                    }
                }
            }

            Writer.Tag(HtmlTextWriterTag.H1, "내부 데이터 정리");

            var index = 0;
            using (Writer.BeginTag(Tag.Ul))
                while (index < InnerTypeList.Count)
                {
                    var inType = InnerTypeList.ElementAt(index++);
                    if (inType.IsArray) inType = inType.GetElementType();
                    if (inType.IsSystem()) continue;

                    using (Writer.BeginTag(Tag.Li))
                    using (Writer.Add(At.Name, inType.GenericName()).BeginTag(Tag.A, inType.GenericName()))
                    {
                        using (Writer.BeginTable(true, Define.Headers))
                        {
                            foreach (var PropertyInfo in inType.GetProperties())
                                if (PropertyInfo.GetCustomAttribute(typeof(ScriptIgnoreAttribute)) != null) continue;
                                else Writer.TagTR(InnerTypeList, PropertyInfo);
                            if (inType.IsEnum)
                                foreach (var EnumValue in inType.GetEnumValues())
                                    using (Writer.BeginTag(Tag.Tr))
                                    {
                                        Writer.Tag(Tag.Td, EnumValue);
                                        Writer.Tag(Tag.Td, (int)EnumValue);
                                        Writer.Tag(Tag.Td, inType.EnumHelp(EnumValue));
                                    }
                        }
                        Writer.Tag(Tag.Br);
                    }
                }

            _APIList = Writer.ReadToEnd();

            return _APIList;
        }
    }

    /// <summary>
    /// ListHelp
    /// </summary>
    public partial class HelpController
    {
        static string _HelpList = null;
        static Dictionary<string, string> _HelpRedirect = new Dictionary<string, string>();

        public object HelpList()
        {
            if (_HelpList != null) return _HelpList;

            var Writer = new TagWriter().Write(Define.Help).Tag(Tag.Br).Tag(Tag.Br);
                
            foreach (var MethodInfo in GetType().GetMethods())
            {
                if (MethodInfo.DeclaringType == typeof(HelpController)) continue;
                if (MethodInfo.DeclaringType != this.GetType()) continue;

                if (MethodInfo.ReturnType == typeof(ActionResult))
                {
                    using (Writer.Add(At.Href, MethodInfo.Name).Add(At.Target, Define.View).BeginTag(Tag.A))
                        Writer.Write(MethodInfo.Name);
                    Writer.Tag(Tag.Br);                    
                    continue;
                }

                var tName = "__" + MethodInfo.Name;

                using (Writer.Add(At.Href, "HelpRedirect/?Method=" + tName).Add(At.Target, Define.View).BeginTag(Tag.A))
                    Writer.Write(MethodInfo.Name);
                Writer.Tag(Tag.Br);

                if (_HelpRedirect.ContainsKey(tName)) continue;

                var tPage = new TagWriter().Write(MethodInfo.Help());
                using (tPage.Add(Define.Action, string.Format("{0}/{1}", URI(), MethodInfo.Name)).Add(Define.Method, Define.Post).BeginTag(Tag.Form))
                using (tPage.BeginTable(false))
                {
                    foreach (var ParameterInfo in MethodInfo.GetParameters())
                        using (tPage.BeginTag(Tag.Tr))
                        {
                            tPage.Tag(Tag.Td, ParameterInfo.Name);
                            using (tPage.BeginTag(Tag.Td))
                                tPage.Add(At.Type, Define.Text).Add(At.Name, ParameterInfo.Name).Tag(Tag.Input);
                            tPage.Tag(Tag.Td, ParameterInfo.ParameterType);
                        }
                    using (tPage.BeginTag(Tag.Tr))
                    {
                        tPage.Tag(Tag.Td);
                        using (tPage.BeginTag(Tag.Td))
                            tPage.Add(At.Type, Define.Submit).Tag(Tag.Input);
                    }
                }
                _HelpRedirect.Add(tName, tPage.ReadToEnd());
            }           
            _HelpList = Writer.ReadToEnd();
            return _HelpList;
        }

        public object HelpRedirect(string Method)
        {
            if (_HelpList == null) HelpList();
            if (_HelpRedirect.ContainsKey(Method)) return _HelpRedirect[Method];
            return null;
        }
    }

    /// <summary>
    /// ListTest
    /// </summary>
    public partial class HelpController
    {
        static string _TestList = null;
        static Dictionary<string, string> _TestRedirect = new Dictionary<string, string>();

        public string TestList()
        {
            if (_TestList != null) return _TestList;

            var Writer = new TagWriter();
            Writer.Add(At.Href, Define.API).Add(At.Target, Define.View).Tag(Tag.A, Define.API).Tag(Tag.Br);
            foreach (var Ctrl in ServerConfig.RestList)
            {
                Writer.Tag(Tag.Br).Write(Ctrl.Key.CtrlName()).Tag(Tag.Br);

                foreach (var MethodInfo in Ctrl.Value)
                {
                    var tName = "__" + MethodInfo.Name;
                    using (Writer.Add(At.Href, "TestRedirect/?Method=" + tName).Add(At.Target, Define.View).BeginTag(Tag.A))
                        Writer.Write(MethodInfo.Name);
                    Writer.Tag(Tag.Br);

                    if (_TestRedirect.ContainsKey(tName)) continue;

                    var tPage = new TagWriter().Write(MethodInfo.Help());
                    using (tPage.Add(Define.Action, string.Format("{0}/TestRedirect2?Route={1}/{2}", URI(), Ctrl.Key.CtrlName(), MethodInfo.Name)).Add(Define.Method, Define.Post).BeginTag(Tag.Form))
                    using (tPage.BeginTable(false))
                    {
                        foreach (var ParameterInfo in MethodInfo.GetParameters())
                            if (ParameterInfo.ParameterType.IsSystem())
                                using (tPage.BeginTag(Tag.Tr))
                                {
                                    tPage.Tag(Tag.Td, ParameterInfo.Name);
                                    using (tPage.BeginTag(Tag.Td))
                                        tPage.Add(At.Type, Define.Text).Add(At.Name, ParameterInfo.Name).Tag(Tag.Input);
                                    tPage.Tag(Tag.Td, ParameterInfo.ParameterType);
                                }
                            else
                            {
                                using (tPage.BeginTag(Tag.Tr))
                                    tPage.Tag(Tag.Td, ParameterInfo.ParameterType.Name).Tag(Tag.Td).Tag(Tag.Td);

                                foreach (var PropertyInfo in ParameterInfo.ParameterType.GetProperties())
                                    using (tPage.BeginTag(Tag.Tr))
                                    {
                                        tPage.Tag(Tag.Td, PropertyInfo.Name);
                                        using (tPage.BeginTag(Tag.Td))
                                            tPage.Add(At.Type, Define.Text).Add(At.Name, PropertyInfo.Name).Tag(Tag.Input);
                                        tPage.Tag(Tag.Td, PropertyInfo.PropertyType);
                                    }
                            }
                        using (tPage.BeginTag(Tag.Tr))
                        {
                            tPage.Tag(Tag.Td);
                            using (tPage.BeginTag(Tag.Td))
                                tPage.Add(At.Type, Define.Submit).Tag(Tag.Input);
                        }
                    }

                    _TestRedirect.Add(tName, tPage.ReadToEnd());
                }

            }            



            _TestList = Writer.ReadToEnd();
            return _TestList;
        }

        public object TestRedirect(string Method)
        {
            if (_TestList == null) TestList();
            if (_TestRedirect.ContainsKey(Method)) return _TestRedirect[Method];
            return null;
        }

        public string TestRedirect2(string Route)
        {
            var PostBodyList = new Dictionary<string, string>();
            foreach (var Key in Request.Form.AllKeys)
                PostBodyList.Add(Key, Request.Form[Key]);
            try
            {
                return new WebClient { Encoding = Encoding.UTF8 }.UploadString(
                  string.Format("http://{0}{1}/{2}", Request.Url.Authority, Request.ApplicationPath, Route),
                  JsonConvert.SerializeObject(PostBodyList));
            }
            catch (WebException ex)
            {
                ex.Response.GetResponseStream().CopyTo(Response.OutputStream);
                return null;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    /// <summary>
    /// ErrorDescription
    /// </summary>
    public partial class HelpController
    {
        internal static IErrorDescription Error = null;

        public string ErrorDefault()
        {
            if (Error == null) return "";
            return ChangeENV2Web(Error.Default());
        }

        public string ErrorExtension()
        {
            if (Error == null) return "";
            return ChangeENV2Web(Error.Extension());
        }

        private string ChangeENV2Web(string inString)
        {
            return inString.Replace(Environment.NewLine, Define.NewLine).Replace(Define.ENVL, Define.WebL).Replace(Define.ENVR, Define.WebR);
        } 
    }

    
}
