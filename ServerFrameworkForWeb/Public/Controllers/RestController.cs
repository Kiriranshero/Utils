using System.Web.Mvc;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System;
using System.Globalization;
using System.Web.Mvc.Async;
using System.Net;


namespace ServerFramework
{

    [RestAuth]        
    [ExceptionHandler(ExceptionType = typeof(Exception))]    
    public abstract class RestController : Controller
    {              
        private static ContentRendererActionInvoker _ActionInvoker = new ContentRendererActionInvoker();
        
        public RestController()
        {
            ActionInvoker = _ActionInvoker;
        }

        protected void Error(string message)
        {
            throw new RestException(message);
        }

        protected void Error<T>(T error, string message = "")
            where T : struct, IConvertible
        {
            throw new RestException(error, message);
        }
    }
    
    public class ContentRendererActionInvoker : AsyncControllerActionInvoker
    {
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            if (actionReturnValue == null) return new EmptyResult();
            return new ObjectResult { Content = actionReturnValue };            
        }
    }
}
