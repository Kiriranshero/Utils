using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public interface ICache
    {
        object GetCache(NameValueCollection header);
        void SetCache(NameValueCollection header, object result);
    }
}
