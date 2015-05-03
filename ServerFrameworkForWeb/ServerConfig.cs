using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServerFramework
{    
    
    public static class ServerConfig
    {
        internal static Dictionary<Type, HashSet<MethodInfo>> RestList = InitRest();
        private static Dictionary<Type, HashSet<MethodInfo>> InitRest()
        {
            var tResult = new Dictionary<Type, HashSet<MethodInfo>>();

            var BaseType = typeof(RestController);
            var BaseMethods = new HashSet<Type>();

            foreach (var Method in BaseType.GetMethods())
                BaseMethods.Add(Method.DeclaringType);
            
            foreach (var inAssambly in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    if (inAssambly.GlobalAssemblyCache) continue;
                    foreach (var inType in inAssambly.GetTypes())
                    {
                        if (inType.IsSubclassOf(BaseType) == false) continue;
                        if (inType.IsAbstract) continue;

                        var tHashSet = new HashSet<MethodInfo>();
                        tResult.Add(inType, tHashSet);

                        foreach (MethodInfo inMethod in inType.GetMethods())
                            if (BaseMethods.Contains(inMethod.DeclaringType) == false)
                                tHashSet.Add(inMethod);
                    }
                }
                catch { }
            return tResult;
        }

        internal static Dictionary<Type, HashSet<MethodInfo>> HelpList = InitHelp();
        private static Dictionary<Type, HashSet<MethodInfo>> InitHelp()
        {
            var tResult = new Dictionary<Type, HashSet<MethodInfo>>();

            var BaseType = typeof(HelpController);
            var BaseMethods = new HashSet<Type>();

            foreach (var Method in BaseType.GetMethods())
                BaseMethods.Add(Method.DeclaringType);

            foreach (var inAssambly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (inAssambly.GlobalAssemblyCache) continue;
                foreach (var inType in inAssambly.GetTypes())
                {
                    if (inType.IsSubclassOf(BaseType) == false) continue;
                    if (inType.IsAbstract) continue;

                    var tHashSet = new HashSet<MethodInfo>();
                    tResult.Add(inType, tHashSet);

                    foreach (MethodInfo inMethod in inType.GetMethods())
                    {
                        if (BaseMethods.Contains(inMethod.DeclaringType)) continue;
                        tHashSet.Add(inMethod);
                    }
                }
            }
            return tResult;
        }

        public static void RegisterFrame(ServerConfigBase config)
        {
            ObjectResult.FinalizerBase = config;

            HelpController.CustomHelp = config as IHelp;
            HelpController.Error = config as IErrorDescription;

            CustomProviderFactory.Serializer = config as ISerializer;
            ExceptionHandlerAttribute.Error = config as IError;

            RestAuthAttribute.Auth = config as ISession;
            if (RestAuthAttribute.Auth != null) RestAuthAttribute.IgnoreAuth.SetIgnore<IgnoreAuthorizeAttribute>();

            RestAuthAttribute.Cache = config as ICache;
            if (RestAuthAttribute.Cache != null)  RestAuthAttribute.IgnoreCache.SetIgnore<IgnoreCacheAttribute>();

            CustomProviderFactory.Crypto = config as ICrypto;
            if (CustomProviderFactory.Crypto != null) CustomProviderFactory.Ignore.SetIgnore<IgnoreCryptoAttribute>();

            foreach (var Provider in ValueProviderFactories.Factories)
            {                
                if (Provider is JsonValueProviderFactory == false) continue;                
                ValueProviderFactories.Factories.Remove(Provider);
                ValueProviderFactories.Factories.Add(new CustomProviderFactory());
                break;
            }

            foreach (var InterfaceInfo in config.GetType().GetInterfaces())
            {
                if (InterfaceInfo.IsGenericTypeDefinition == false) continue;
                if (InterfaceInfo.GetGenericTypeDefinition() == typeof(IFinalizer<>))
                    ObjectResult.Finalizer.Add(InterfaceInfo.GetGenericArguments()[0], InterfaceInfo.GetMethods()[0]);             
                
            }
        }
      
        
        #region Help

        public static string Root { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        private static string _HelpPath = null;

        public static string HelpPath
        {
            get
            {
                if (_HelpPath == null) _HelpPath = Root + "Bin";
                return _HelpPath;
            }
            set
            {
                _HelpPath = value;
            }
        }
        
        public static bool EnableHelp { get; set; }
        
        #endregion

       
    }
}
