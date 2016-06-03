using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OOUmbracoElmah
{
    public partial class Elmah : System.Web.UI.UserControl
    {
        private umbraco.BusinessLogic.User admin_user;

        protected void Page_Load(object sender, EventArgs e)
        {
            admin_user = umbraco.BusinessLogic.User.GetCurrent();
            if (admin_user == null)
            {
                Response.Write(@"<script type='text/javascript'>top.document.location.href = '~/umbraco/login.aspx';</script>");
                Response.End();
            }
        }
    }
}