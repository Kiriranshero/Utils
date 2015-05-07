using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    public class JsonRequest : WebClient
    {
        public int TimeOut { get; set; }
        public string ResultMessage { get; private set; }
        public Exception Error { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var Request = base.GetWebRequest(address) as HttpWebRequest;
            Request.ContentType = "application/json; charset=utf-8";
            if (TimeOut > 0) Request.Timeout = TimeOut;
            Request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return Request;
        }

        public T UploadObject<T>(string address, object inObject = null)
        {
            try
            {
                if (inObject == null) ResultMessage = this.DownloadString(address);
                else ResultMessage = this.UploadString(address, inObject as string ?? JsonConvert.SerializeObject(inObject));
                if (string.IsNullOrEmpty(ResultMessage) == false)
                    return JsonConvert.DeserializeObject<T>(ResultMessage);
            }
            catch (Exception ex)
            {
                Error = ex;
            }
            return default(T);
        }

        public T DownloadObject<T>(string address)
        {
            return UploadObject<T>(address);
        }
    }
}
