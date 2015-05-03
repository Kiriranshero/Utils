using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public interface ISession
    {
        bool SessionCheck(NameValueCollection headers, IDictionary items);        
    }
}
