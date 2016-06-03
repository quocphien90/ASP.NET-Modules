using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace OgilvyOneOAuth
{
    public class OAuthGoogle
    {
        private string ClientID { get; set; }
        private string ClientSecret { get; set; }
        private string RedirectUrl { get; set; }
        private string RequestURL { get; set; }

        public string AccessToken { get; set; }

        public OAuthGoogle()
        {
            if (ConfigurationSettings.AppSettings["GoogleClientID"] != null)
            {
                ClientID = ConfigurationSettings.AppSettings["GoogleClientID"].ToString();
            }
            if (ConfigurationSettings.AppSettings["GoogleClientSecret"] != null)
            {
                ClientSecret = ConfigurationSettings.AppSettings["GoogleClientSecret"].ToString();
            }
            if (ConfigurationSettings.AppSettings["OAuthCallbackURL"] != null)
            {
                RedirectUrl = ConfigurationSettings.AppSettings["OAuthCallbackURL"].ToString();
            }
        }

        public OAuthGoogle(string sClientID, string sClientSecret, string sRedirectUrl)
        {
            ClientID = sClientID;
            ClientSecret = sClientSecret;
            RedirectUrl = sRedirectUrl;
        }

        public string MakeRequestAuth()
        {
            OAuthGoogle objGoogle = new OAuthGoogle();
            objGoogle.RequestURL = "https://accounts.google.com/o/oauth2/auth?";

            NameValueCollection objQueryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            objQueryString["client_id"] = objGoogle.ClientID;
            objQueryString["response_type"] = "token";
            objQueryString["scope"] = "openid profile email";
            objQueryString["redirect_uri"] = objGoogle.RedirectUrl;

            return objGoogle.RequestURL + objQueryString.ToString();
        }

        public string GetAccessToken(string sCurrentUrl = "")
        {
            string sAccessToken = "";
            if (sCurrentUrl == "")
                sCurrentUrl = HttpContext.Current.Request.Url.ToString().ToLower();

            if (sCurrentUrl.Contains("#access_token"))
            {
                string[] sTmp = sCurrentUrl.Split(new char[] { '#' });
                string sQueries = sTmp[1].ToString();
                NameValueCollection objQueryString = System.Web.HttpUtility.ParseQueryString(sQueries);
                sAccessToken = objQueryString["access_token"].ToString();
            }

            this.AccessToken = sAccessToken;
            return sAccessToken;
        }


        public OAuthUserInfo GetUserProfile(out string sErrorMessage)
        {
            sErrorMessage = "";
            if (this.AccessToken == "" || this.AccessToken.Length < 10)
            {
                return null;
            }

            OAuthUserInfo objUser = new OAuthUserInfo();
            this.RequestURL = "https://www.googleapis.com/oauth2/v1/userinfo?";

            NameValueCollection objQueryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            objQueryString["alt"] = "json";
            objQueryString["access_token"] = this.AccessToken;

            string sJsonData = objUser.MakeRequest(this.RequestURL, out sErrorMessage, "GET", objQueryString.ToString(), null);

            if (sJsonData == "" || sErrorMessage != "")
            {
                return null;
            }

            objUser.Type = UserType.Google;
            JObject objData = JObject.Parse(sJsonData);
            if (objData != null && objData["id"] != null)
            {
                objUser.ID = objData["id"].ToString();
                objUser.Name = objData["name"].ToString();
                objUser.Email = objData["email"].ToString();
                if (objData["gender"] != null)
                {
                    objUser.Gender = (objData["gender"].ToString().ToLower() == "male" ? UserGender.Male : UserGender.Female);
                }
                objUser.NickName = objData["email"].ToString().Substring(0, objData["email"].ToString().IndexOf("@"));
            }

            //HttpContext.Current.Response.Write(sJsonData);
            return objUser;
        }
    }
}