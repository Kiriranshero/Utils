using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class JsonRequest : WebClient
    {
        public static int TimeOut = 9000000;

        protected override WebRequest GetWebRequest(Uri address)
        {
            var Request = base.GetWebRequest(address) as HttpWebRequest;
            Request.ContentType = "application/json; charset=utf-8";
            Request.Timeout = TimeOut;
            Request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return Request;
        }

        public string ErrorMessage { get; set; }
        public Exception Error { get; set; }

        public T UploadObject<T>(string address, object inObject)
            where T : class
        {
            try
            {
                ErrorMessage = this.UploadString(address, inObject as string ?? JsonConvert.SerializeObject(inObject));
                if (string.IsNullOrEmpty(ErrorMessage)) return null;
                return JsonConvert.DeserializeObject<T>(ErrorMessage);
            }
            catch (Exception ex)
            {
                Error = ex;
                return null;
            }
        }

        public T DownloadObject<T>(string address)
           where T : class
        {
            try
            {
                ErrorMessage = this.DownloadString(address);
                if (string.IsNullOrEmpty(ErrorMessage)) return null;
                return JsonConvert.DeserializeObject<T>(ErrorMessage);
            }
            catch (Exception ex)
            {
                Error = ex;
                return null;
            }

        }
    }
}
