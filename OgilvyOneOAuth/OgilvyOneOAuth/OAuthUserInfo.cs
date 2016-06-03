using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace OgilvyOneOAuth
{
    public class OAuthUserInfo
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public UserGender Gender { get; set; }
        public UserType Type { get; set; }

        public string MakeRequest(string sUrl, out string sErrorMessage, string sMethod = "POST", string sDataString = "", WebHeaderCollection objHeader = null)
        {
            string response_text = "";
            sErrorMessage = "";
            try
            {
                HttpWebRequest objRequest;
                if (sMethod == "POST")
                {
                    byte[] data = Encoding.UTF8.GetBytes(sDataString);
                    objRequest = (HttpWebRequest)WebRequest.Create(sUrl);
                    objRequest.Method = sMethod;
                    objRequest.ContentLength = data.Length;
                    objRequest.ContentType = "application/x-www-form-urlencoded";

                    Stream dataStream = objRequest.GetRequestStream();
                    try
                    {
                        dataStream.Write(data, 0, data.Length);
                        dataStream.Close();
                    }
                    catch (Exception) { }
                    finally
                    {
                        dataStream.Close();
                    }
                }
                else
                {
                    if (sDataString != "")
                    {
                        sUrl += (sUrl.Contains("?") ? "" : "?") + sDataString;
                    }
                    objRequest = (HttpWebRequest)WebRequest.Create(sUrl);
                }

                if (objHeader != null && objHeader.Count > 0)
                {
                    objRequest.Headers = objHeader;
                }

                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    response_text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
            }
            return response_text;
        }
    }

    public enum UserType
    {
        Yahoo,
        Google
    };

    public enum UserGender
    {
        Male,
        Female
    };
}