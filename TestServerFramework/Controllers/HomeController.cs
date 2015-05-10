using ServerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestServerFramework.Controllers
{
    public class HomeController : HelpController
    {
        public object Test1(string inTest)
        {
            return inTest;
        }

        public object Tesd2(string inTest)
        {
            return inTest;
        }
    }
}