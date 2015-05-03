using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServerFramework
{
    class RestAuthAttribute : AuthorizeAttribute
    {
        internal static ISession Auth = null;
        internal static IgnoreWorker IgnoreAuth = new IgnoreWorker();

        internal static ICache Cache = null;
        internal static IgnoreWorker IgnoreCache = new IgnoreWorker();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return false;            
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            do
            {
                if (Cache == null) break;
                if (IgnoreCache.IsDefined(filterContext.RouteData)) break;
                var CacheResult = Cache.GetCache(filterContext.HttpContext.Request.Headers);
                if (CacheResult == null) break;
                filterContext.Result = new ObjectResult { Content = CacheResult, Cached = true };                
                return;
            } while (false);
            do
            {
                if (Auth == null) break;
                if (IgnoreAuth.IsDefined(filterContext.RouteData)) break;
                if (Auth.SessionCheck(filterContext.HttpContext.Request.Headers, filterContext.HttpContext.Items)) break;
                HandleUnauthorizedRequest(filterContext);
            } while (false);
        }
    }
}
