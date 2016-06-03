using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace OOSecurityFilter
{
    public class WebSecurityFilter : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(OnBeginRequest);
            application.Error += new EventHandler(OnErrorHanndle);
        }

        public void OnBeginRequest(object sender, EventArgs e)
        {
            if (!HttpContext.Current.Request.Url.ToString().Contains("aspxerrorpath=") 
                && !HttpContext.Current.Request.Url.ToString().ToLower().Contains("/umbraco")
                && !HttpContext.Current.Request.Url.ToString().ToLower().Contains("/umbraco_client") 
                && umbraco.BusinessLogic.User.GetCurrent() == null)
            {
                

                if (!isValidRequest())
                {
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session.Clear();
                        HttpContext.Current.Session.Abandon();
                    }

                    if (FormsAuthentication.IsEnabled)
                    {
                        FormsAuthentication.SignOut();
                    }

                    //DateTime expired = DateTime.Now.AddSeconds(1);
                    //common_logic.set_cookie("member_id", "", expired);
                    //common_logic.set_cookie("member_first_name", "", expired);
                    //common_logic.set_cookie("member_last_name", "", expired);
                    //common_logic.set_cookie("member_nickname", "", expired);
                    //common_logic.set_cookie("member_guid", "", expired);
                    //common_logic.set_cookie("member_email", "", expired);
                    //common_logic.set_cookie("member_mobile", "", expired);
                    //common_logic.set_cookie("member_address", "", expired);
                    //common_logic.set_cookie("member_province", "", expired);
                    //common_logic.set_cookie("member_dob", "", expired);
                    //common_logic.set_cookie("member_status", "", expired);


                    HttpContext.Current.Response.Redirect("/error.html");
                }
            }
        }

        public void OnErrorHanndle(object sender, EventArgs e)
        {
            
        }

        public void Dispose()
        { }

        protected bool isValidRequest()
        {
            bool is_valid = true;

            if (IsValidSQLInjection(HttpContext.Current.Request.Url.ToString()))
            {
                is_valid = false;
            }

            if (is_valid)
            {
                foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                {
                    string value = HttpContext.Current.Request.Form[key].ToString();
                    if (IsValidSQLInjection(value))
                    {
                        is_valid = false;
                        break;
                    }
                }

            }

            if (is_valid)
            {
                foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
                {
                    string value = HttpContext.Current.Request.QueryString[key].ToString();
                    if (IsValidSQLInjection(value))
                    {
                        is_valid = false;
                        break;
                    }
                }
            }

            if (is_valid)
            {
                foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
                {
                    if (HttpContext.Current.Request[key] != null)
                    {
                        string value = HttpContext.Current.Request.Cookies[key].Value.ToString();
                        if (IsValidSQLInjection(value) || IsValidSQLInjection(HttpUtility.UrlDecode(value)))
                        {
                            is_valid = false;
                            break;
                        }
                    }
                }
            }

            //if (is_valid)
            //{
            //    foreach (string key in HttpContext.Current.Request.Headers.AllKeys)
            //    {
            //        if (key != "Cookie" && IsValidSQLInjection(HttpContext.Current.Request.Headers[key].ToString()))
            //        {
            //            is_valid = false;
            //            break;
            //        }
            //    }
            //}

            return is_valid;
        }

        protected bool IsValidSQLInjection(String Text)
        {
            Text = Text.ToLower();
            if (Text.Contains("xp_") || Text.Contains("union") || Text.Contains("uni/**/on")
                || Text.Contains("'\"") || Text.Contains(");|]*")
                || Text.Contains("{%0d") || Text.Contains("|]*{")
                || Text.Contains("\";") || Text.Contains("';")
                || Text.Contains("' or") || Text.Contains("\" or")
                || Text.Contains("' and") || Text.Contains("\" and")
                || Text.Contains(";--") || Text.Contains("nvarchar")
                || Text.Contains("-1 or") || Text.Contains("1 or")
                || Text.Contains("-1 and") || Text.Contains("1 and")
                || Text.Contains("sleep") || Text.Contains("; waitfor delay")
                || Text.Contains("<script") || Text.Contains("</script>")
                || Text.Contains(">prompt(") || Text.Contains("()&%<acx>")
                || Text.Contains("1'1") || Text.Contains("():;")
                )
            {
                return true;
            }
            return false;
        }
    }
}