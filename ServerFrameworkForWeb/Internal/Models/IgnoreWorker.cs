using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Reflection;

namespace ServerFramework
{
    class IgnoreWorker : Dictionary<string, HashSet<string>>       
    {
        public IgnoreWorker() : base(StringComparer.OrdinalIgnoreCase) { }

        public bool IsDefined(RouteData routeData)
        {
            var Ctrl = routeData.Ctrl();
            var Result = ContainsKey(Ctrl);
            if (Result) Result = this[Ctrl].Contains(routeData.Action());
            return Result;
        }

        internal void SetIgnore<T>()
            where T : Attribute
        {
            if (Count != 0) return;

            foreach (var inControl in ServerConfig.RestList)
            {
                var CtrlName = inControl.Key.CtrlName();

                foreach (var inMethod in inControl.Value)
                {
                    if (inMethod.GetCustomAttribute<T>(true) == null) continue;
                    if (ContainsKey(CtrlName) == false) Add(CtrlName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                    this[CtrlName].Add(inMethod.Name);
                }
            }
        }
    }
}
