using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace ServerFramework
{
    class TagWriter : IDisposable
    {
        private HtmlTextWriter writer = null;

        private Stream BaseStream = null;

        public TagWriter(HttpResponseBase response)
        {
            writer = new HtmlTextWriter(response.Output);
        }

        public TagWriter()
        {
            BaseStream = new CustomStream();
            writer = new HtmlTextWriter(new StreamWriter(BaseStream) { AutoFlush = true });
        }

        public TagWriter Add(HtmlTextWriterAttribute attr, string value)
        {
            writer.AddAttribute(attr, value);            
            return this;
        }

        public TagWriter Add(string attr, string value)
        {
            writer.AddAttribute(attr, value);
            return this;
        }

        public TagWriter Write(object value)
        {
            writer.Write(value);
            return this;
        }

        public TagWriter Tag(HtmlTextWriterTag tag, params object[] values)
        {
            if (tag == HtmlTextWriterTag.Br)
                writer.Write("<br />");
            else
            {
                writer.RenderBeginTag(tag);
                foreach (var value in values)
                    writer.Write(value);
                writer.RenderEndTag();
            }
            return this;
        }

        public string ReadToEnd()
        {
            if (BaseStream == null) return "";
            BaseStream.Position = 0;
            using (var Reader = new StreamReader(BaseStream))
                return Reader.ReadToEnd();
        }

      
        public TagWriter BeginTag(HtmlTextWriterTag tag, params object[] values)
        {
            writer.RenderBeginTag(tag);
            foreach (var value in values)
                writer.Write(value);
            return this;
        }

        public void Dispose()
        {
            writer.RenderEndTag();
        }

        public TagWriter BeginTable(bool border, params object[] values)
        {
            if(border) writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            using (BeginTag(HtmlTextWriterTag.Tr))
                foreach (var value in values)
                    Tag(HtmlTextWriterTag.Th, value);
            return this;
        }

        
        

        public void TagTR(HashSet<Type> InnerTypeList, PropertyInfo PropertyInfo)
        {
            using (BeginTag(HtmlTextWriterTag.Tr))
            {
                Tag(HtmlTextWriterTag.Td, PropertyInfo.Name);
                if (PropertyInfo.PropertyType.IsSystem())
                    Tag(HtmlTextWriterTag.Td, PropertyInfo.PropertyType.GenericName());
                else
                    using (BeginTag(HtmlTextWriterTag.Td))
                        TD(InnerTypeList, PropertyInfo.PropertyType);
                Tag(HtmlTextWriterTag.Td, PropertyInfo.Help());
            }
        }

        void TD(HashSet<Type> InnerTypeList, Type type)
        {
            if (type.IsSystem())
            {
                writer.Write(type.Name);
                return;
            }

            if (type.IsArray)
            {
                writer.Write("[");
                TD(InnerTypeList, type.GetElementType());
                writer.Write("]");
            }
            else if (type.IsGenericType)
            {
                if (type.Name.Contains("List"))
                {
                    writer.Write("[");
                    TD(InnerTypeList, type.GenericTypeArguments[0]);
                    writer.Write("]");
                }
                else if (type.Name.Contains("Dictionary"))
                {
                    writer.Write("[");
                    TD(InnerTypeList, type.GenericTypeArguments[0]);
                    writer.Write(",");
                    TD(InnerTypeList, type.GenericTypeArguments[1]);
                    writer.Write("]");
                }
                else
                {
                    InnerTypeList.Add(type);
                    var Name = type.GenericName();
                    Add(HtmlTextWriterAttribute.Href, "#" + Name).Tag(HtmlTextWriterTag.A, Name);
                }
            }
            else
            {
                InnerTypeList.Add(type);
                Add(HtmlTextWriterAttribute.Href, "#" + type.Name).Tag(HtmlTextWriterTag.A, type.Name);
            }
        }
    }
}
