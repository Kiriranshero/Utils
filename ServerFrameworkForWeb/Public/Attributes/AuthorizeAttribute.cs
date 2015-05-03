using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public class IgnoreAuthorizeAttribute : Attribute
    {
    }

    public class AutherizeLoginAttribute : IgnoreAuthorizeAttribute
    {
    }
}
