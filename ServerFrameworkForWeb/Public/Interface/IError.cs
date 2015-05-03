using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public interface IError        
    {
        void SystemError(IDictionary items, Exception ex, string message = null);
        object EnumError(object error, string message);
    }

    public interface IErrorDescription
    {
        string Default();

        string Extension();
    }


}
