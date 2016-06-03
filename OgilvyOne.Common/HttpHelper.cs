using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OgilvyOne.Common
{
    public class HttpHelper
    {
        public static string GetIPAddress(HttpRequestBase request)
        {
            string ip;
            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch { ip = null; }

            return ip == "::1" ? "127.0.0.1" : ip;
        }

        public static string GetUrl()
        {
            string url = HttpContext.Current.Request.RawUrl;
            string[] arr = url.Split('?');
            string retUrl = "";
            if (arr.Length > 0)
                retUrl = arr[0];

            retUrl = retUrl.TrimEnd('/') + "/";
            return retUrl;
        }
    }
}
