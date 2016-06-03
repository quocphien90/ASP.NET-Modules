using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OgilvyOneOAuth
{
    public partial class OAuthCallback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthUserInfo objUserInfo;
            string sAccessToken = "";
            string sYahooGuid = "";
            string sErrorMessage = "";


            string sOAuthUrl = Request.QueryString["OAuthUrl"].ToString();
            if (sOAuthUrl != "")
            {
                if (sOAuthUrl.Contains("xoauth_yahoo_guid"))
                {
                    OAuthYahoo objYahoo = new OAuthYahoo();
                    sAccessToken = objYahoo.GetAccessToken(sOAuthUrl);
                    sYahooGuid = objYahoo.YahooGuid;
                    objUserInfo = objYahoo.GetUserProfile(out sErrorMessage);
                }
                else
                {
                    OAuthGoogle objGoogle = new OAuthGoogle();
                    sAccessToken = objGoogle.GetAccessToken(sOAuthUrl);
                    objUserInfo = objGoogle.GetUserProfile(out sErrorMessage);
                }
                
                if (objUserInfo != null && objUserInfo.ID != "")
                {
                    Response.Write("ID: " + objUserInfo.ID);
                    Response.Write("Email" + objUserInfo.Email);
                    Response.Write("Name: " + objUserInfo.Name);
                    Response.Write("Type: " + objUserInfo.Type.ToString());
                    Response.Write("Token: " + sAccessToken);
                    Response.Write("Guid: " + sYahooGuid);
               }
            }
            else
            {
                Response.Write("Need to approve permission.");
            }
        }
    }
}