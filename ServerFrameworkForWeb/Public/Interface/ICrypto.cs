using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public interface ICrypto
    {
        Stream Decrypt(NameValueCollection header, IDictionary items, Stream body);
        void Encrypt(IDictionary items, Stream source, Stream result);
    }   
}
