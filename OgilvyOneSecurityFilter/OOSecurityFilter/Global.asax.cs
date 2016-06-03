using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;
using System.Net;
using System.Text;

namespace OOSecurityFilter
{
    public class Global : Umbraco.Web.UmbracoApplication
    {

        protected void Application_Error(object sender, EventArgs e)
        {
            base.Application_Error(sender, e);

            var ex = Server.GetLastError();
            var isHandled = false;
            var httpEx = ex as HttpException;
            if (httpEx != null)
            {
                if (httpEx.GetHttpCode() == (int)HttpStatusCode.NotFound)
                {
                    HandleError404();
                    isHandled = true;
                }
                else
                {
                    HandleError();
                }
            }

            if (!isHandled)
                HandleError();
        }

        /// <summary>
        /// Handles the 404 error - page not found.
        /// </summary>
        private void HandleError404()
        {
            Response.Clear();
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.StatusDescription = "Not Found";
            Response.Redirect("/page-not-found.aspx");
            Response.End();
            Server.ClearError();
        }

        /// <summary>
        /// Handles the generic error - Renders the Error page.
        /// </summary>
        private void HandleError()
        {
            Response.Clear();
            Response.Redirect("/badrequest.html");
            Response.End();

            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }

            try
            {
                HttpContext.Current.Request.Cookies.Clear();
                foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
                {
                    if (HttpContext.Current.Request.Cookies[key] != null)
                    {
                        HttpContext.Current.Request.Cookies[key].Expires = DateTime.Now.AddDays(-1);
                    }
                }

                FormsAuthentication.SignOut();
            }
            catch
            {
            }

            Server.ClearError();
        }

        /// <summary>
        /// Handles the MVC Validation error - Send back a JSON object with extra information about the error.
        /// </summary>
        /// <param name="httpValidationException">The exception</param>
        private void HandleMvcValidationError(HttpRequestValidationException httpValidationException)
        {
            Response.Clear();
            Response.TrySkipIisCustomErrors = true;
            Response.ContentType = "application/json";
            Response.ContentEncoding = Encoding.UTF8;

            //var errorData = ValidationErrorUtility.GetValidationError(httpValidationException);
            //var errorJsonData = new JavaScriptSerializer().Serialize(errorData);
            if (httpValidationException.InnerException != null)
                Response.Write(httpValidationException.InnerException.ToString());
            Response.End();
            Server.ClearError();
        }
    }
}