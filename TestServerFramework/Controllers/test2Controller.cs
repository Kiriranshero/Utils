﻿using ServerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestServerFramework.Controllers
{
    public class test2Controller : RestController
    {
        // GET: Test
        public object Test(int inValue)
        {
            return inValue;
        }
    }
}