using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServerFramework
{
    class ExceptionHandlerAttribute : HandleErrorAttribute
    {
        internal static IError Error = null;

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is RestException)
            {
                var ex = filterContext.Exception as RestException;
                object Result = filterContext.Exception.Message;
                if (Error != null) Result = Error.EnumError(ex.RestError, ex.Message);
                filterContext.Result = new ObjectResult { Content = Result };
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is HttpException)
            {
                if (Error != null) Error.SystemError(filterContext.HttpContext.Items, filterContext.Exception.InnerException);
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.ExceptionHandled = true;
            }
            else
            {
                if (Error != null) Error.SystemError(filterContext.HttpContext.Items, filterContext.Exception.InnerException);
                base.OnException(filterContext);
            }
        }
    }
}
