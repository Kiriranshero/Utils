using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public enum DateFormat
    {
        IsoDateFormat = DateFormatHandling.IsoDateFormat,
        MicrosoftDateFormat = DateFormatHandling.MicrosoftDateFormat,
        UnixDateFormat,
    }
}
