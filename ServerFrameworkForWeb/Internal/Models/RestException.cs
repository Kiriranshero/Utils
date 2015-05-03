using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    class RestException : ExternalException
    {
        public object RestError { get; private set; }

        public RestException(object error, string message = "")
            : base(message)
        {
            RestError = error;
        }

        public RestException(string inMessage) : base(inMessage) { }
    }
}
