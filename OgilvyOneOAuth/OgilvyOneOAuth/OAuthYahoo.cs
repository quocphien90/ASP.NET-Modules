using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;

namespace OgilvyOneOAuth
{
    public class OAuthYahoo
    {
        private string ClientID { get; set; }
        private string ClientSecret { get; set; }
        private string RedirectUrl { get; set; }
        private string RequestURL { get; set; }

        public string AccessToken { get; set; }

        public string YahooGuid { get; set; }

        public OAuthYahoo()
        {
            if (ConfigurationSettings.AppSettings["YahooConsumerKey"] != null)
            {
                ClientID = ConfigurationSettings.AppSettings["YahooConsumerKey"].ToString();
            }
            if (ConfigurationSettings.AppSettings["YahooConsumerSecret"] != null)
            {
                ClientSecret = ConfigurationSettings.AppSettings["YahooConsumerSecret"].ToString();
            }
            if (ConfigurationSettings.AppSettings["OAuthCallbackURL"] != null)
            {
                RedirectUrl = ConfigurationSettings.AppSettings["OAuthCallbackURL"].ToString();
            }
        }

        public string MakeRequestAuth()
        {
            OAuthYahoo objYahoo = new OAuthYahoo();
            objYahoo.RequestURL = "https://api.login.yahoo.com/oauth2/request_auth?";

            NameValueCollection objQueryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            objQueryString["client_id"] = objYahoo.ClientID;
            objQueryString["response_type"] = "token";
            objQueryString["language"] = "en-us";
            objQueryString["redirect_uri"] = objYahoo.RedirectUrl;

            return objYahoo.RequestURL + objQueryString.ToString();
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
                this.YahooGuid = objQueryString["xoauth_yahoo_guid"].ToString();
            }

            this.AccessToken = sAccessToken;
            return sAccessToken;
        }

        public OAuthUserInfo GetUserProfile(out string sErrorMessage)
        {
            sErrorMessage = "";
            if (this.AccessToken == "" || this.YahooGuid == "")
            {
                return null;
            }

            OAuthUserInfo objUser = new OAuthUserInfo();
            this.RequestURL = "https://social.yahooapis.com/v1/user/" + this.YahooGuid + "/profile?";

            NameValueCollection objQueryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            objQueryString["format"] = "json";

            WebHeaderCollection objHeader = new WebHeaderCollection();
            objHeader.Add("Authorization", "Bearer " + this.AccessToken);

            string sJsonData = objUser.MakeRequest(this.RequestURL, out sErrorMessage, "GET", objQueryString.ToString(), objHeader);

            //HttpContext.Current.Response.Write("RequestURL:" + this.RequestURL + "<br/>");
            //HttpContext.Current.Response.Write("sJsonData:" + sJsonData + "<br/>");
            //HttpContext.Current.Response.Write("sErrorMessage:" + sErrorMessage + "<br/>");
            if (sJsonData == "" || sErrorMessage != "")
            {
                return null;
            }

            objUser.Type = UserType.Yahoo;
            JObject objData = JObject.Parse(sJsonData);
            if (objData != null && objData["profile"] != null)
            {
                JObject objProfile = (JObject)objData["profile"];

                objUser.ID = objProfile["guid"].ToString().ToLower();
                objUser.Name = objProfile["familyName"].ToString() + " " + objProfile["givenName"].ToString();
                objUser.NickName = objProfile["nickname"].ToString();

                if (objProfile["emails"] != null)
                {
                    JArray objEmails = (JArray)objProfile["emails"];
                    objUser.Email = objEmails[0]["handle"].ToString();
                }
                else
                {
                    objUser.Email = objUser.ID.ToString() + "@yahoo.com";
                }

                if (objProfile["gender"] != null)
                {
                    objUser.Gender = (objProfile["gender"].ToString().ToLower() == "f" ? UserGender.Female : UserGender.Male);
                }

            }

            //HttpContext.Current.Response.Write(sJsonData);
            return objUser;
        }
    }
}