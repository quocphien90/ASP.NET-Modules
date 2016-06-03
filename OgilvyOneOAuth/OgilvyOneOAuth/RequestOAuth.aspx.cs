using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OgilvyOneOAuth
{
    public partial class RequestOAuth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthGoogle objGoogle = new OAuthGoogle();
            OAuthYahoo objYahoo = new OAuthYahoo();

            Response.Write("GoogleOAuthUrl: " + objGoogle.MakeRequestAuth() + "<br/>");
            Response.Write("YahooOAuthUrl: " + objYahoo.MakeRequestAuth() + "<br/>");
        }
    }
}