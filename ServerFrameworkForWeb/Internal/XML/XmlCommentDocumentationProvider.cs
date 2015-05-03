using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;

using System.IO;

namespace ServerFramework
{
    class XmlCommentDocumentationProvider
    {
        Dictionary<string, XPathNavigator> _DocNavis = new Dictionary<string, XPathNavigator>();   

        private const string _Assembly = "/doc/assembly/name";
        private const string _methodExpression = "/doc/members/member[@name='M:{0}']";
        private const string _classExpression = "/doc/members/member[@name='T:{0}']";
        private const string _propertyExpression = "/doc/members/member[@name='P:{0}.{1}']";
        private const string _enumExpression = "/doc/members/member[@name='F:{0}.{1}']";
        private const string _summary = "summary";
        private const string _return = "returns";
        private const string _Param = "param[@name='{0}']";

        private static Regex nullableTypeNameRegex = new Regex(@"(.*\.Nullable)" + Regex.Escape("`1[[") + "([^,]*),.*");

        public XmlCommentDocumentationProvider(string documentRoot)
        {
            if (Directory.Exists(documentRoot) == false) return;

            foreach (var inPath in Directory.GetFiles(documentRoot, "*.xml"))
            {
                try 
	            {	        
                    var tNavi =  new XPathDocument(inPath).CreateNavigator();
		            var tNode = tNavi.SelectSingleNode(_Assembly);
                    if (_DocNavis.ContainsKey(tNode.Value)) _DocNavis[tNode.Value] = tNavi;
                    else _DocNavis.Add(tNode.Value, tNavi);
	            }
	            catch { }
            }
        }

        #region ClassInfo
       
        XPathNavigator Finder(string FileName, string Express1, string Express2 = null)
        {
            if (_DocNavis.ContainsKey(FileName))
            {
                var Member1 = _DocNavis[FileName].SelectSingleNode(Express1);
                if (Member1 != null) return Member1.SelectSingleNode(Express2 ?? _summary);
            }
            return null;           
        }

        public XPathNavigator GetClass(Type inType, string Name = null)
        {
            return Finder(inType.Assembly.GetName().Name,
                string.Format(_classExpression, inType.FullName), Name);          
        }

        #endregion

        #region MethodInfo

        internal XPathNavigator GetMethod(MethodInfo inMethod, string Name = null)
        {
            return Finder(inMethod.ReflectedType.Assembly.GetName().Name,
                string.Format(_methodExpression, GetMethodMemberName(inMethod)), Name);
        }

        internal XPathNavigator GetMethodParams(MethodInfo inMethod, ParameterInfo inParam)
        {
            return Finder(inMethod.ReflectedType.Assembly.GetName().Name,
               string.Format(_methodExpression, GetMethodMemberName(inMethod)),
               string.Format(_Param, inParam.Name));
        }

        internal XPathNavigator GetMethodReturn(MethodInfo inMethod)
        {
            return Finder(inMethod.ReflectedType.Assembly.GetName().Name,
              string.Format(_methodExpression, GetMethodMemberName(inMethod)), _return);
        }    

        private static string GetMethodMemberName(MethodInfo method)
        {
            string name = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            var parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => ProcessTypeName(param.ParameterType.FullName)).ToArray();
                name += string.Format("({0})", string.Join(",", parameterTypeNames));
            }
            return name;
        }

        private static string ProcessTypeName(string typeName)
        {
            var result = nullableTypeNameRegex.Match(typeName);
            if (result.Success) return string.Format("{0}{{{1}}}", result.Groups[1].Value, result.Groups[2].Value);
            return typeName;
        }

        #endregion

        #region Enum

        internal XPathNavigator GetEnum(Type inType, object inEnum)               
        {
            return Finder(inType.Assembly.GetName().Name,
                string.Format(_enumExpression, inType.FullName, inEnum));           
        }

        internal XPathNavigator GetEnumValue(Type inType, object inProperty)
        {
            return Finder(inType.Assembly.GetName().Name,
               string.Format(_enumExpression, inType.FullName, inProperty));           
        }
        
        #endregion

        public XPathNavigator GetProperty(PropertyInfo inType)
        {
            return Finder(inType.ReflectedType.Assembly.GetName().Name,
                string.Format(_propertyExpression, Regex.Replace(inType.DeclaringType.ToString(), "\\[.*\\]", ""), inType.Name));
        }        
    }
}
