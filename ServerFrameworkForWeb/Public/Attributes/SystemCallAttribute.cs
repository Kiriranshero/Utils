using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework.Public.Attributes
{
    public class SystemCallAttribute : Attribute
    {
        public SystemCallAttribute(string header, string value)        
        {
            SystemAttribute.CheckHeader = header;
            SystemAttribute.CheckValue = value;
        }
    }
}
