using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServerFramework
{
    class SystemAttribute : ActionFilterAttribute
    {
        internal static string CheckHeader { get; set; }

        internal static string CheckValue { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Header = filterContext.HttpContext.Request.Headers;
            if (string.IsNullOrEmpty(CheckHeader) == false)
                if (string.IsNullOrEmpty(Header[CheckHeader]) || (Header[CheckHeader] != CheckValue))
                    throw new HttpException(404, "Page not found");
            base.OnActionExecuting(filterContext);
        }
    }
}
