using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    class Define
    {
        public const string Action = "action";
        public const string Method = "method";
        public const string Help = "Help";
        public const string View = "View";
        public const string Post = "post";
        public const string Text = "text";
        public const string Submit = "submit";

        public const string Request = "Request";
        public const string Response = "Response";
        
        public const string NewLine = "<br />";

        public const string ENVL = "<";
        public const string ENVR = ">";

        public const string WebL = "&lt";
        public const string WebR = "&gt";
        public const string API = "API";

        public static string[] Headers = { "필드명", "데이터형", "설명" };
    }
}
