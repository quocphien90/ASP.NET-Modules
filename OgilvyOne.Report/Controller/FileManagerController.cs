using Newtonsoft.Json.Linq;
using OgilvyOne.Backend.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;

namespace OgilvyOne.Report.Controller
{
   
    public class FileManagerController : Umbraco.Web.Mvc.SurfaceController
    {
        //[HttpPost]
        public JObject Upload(HttpPostedFileBase file, int ID, string Path, string checkSum)
        {
            JObject kq = new JObject();
            try
            {
                var user = umbraco.BusinessLogic.User.GetCurrent();
                if (user == null)
                {

                    kq.Add("HasError", true);
                    kq.Add("ErrorCode", 1);
                    kq.Add("ErrorHtml", null);
                    kq.Add("Data", "/umbraco/");
                    kq.Add("Error", "User have to logged in");
                    
                    return kq;
                }
                var config = FileManagerHelper.GetFileManagerConfig(ID);
                if (config == null)
                {
                    kq.Add("HasError", true);
                    kq.Add("ErrorCode", 2);
                    kq.Add("ErrorHtml", "Cannot found");
                    kq.Add("Data", false);
                    kq.Add("Error", "Cannot found");

                }
                if (file != null && file.ContentLength > 0)
                {
                    FileManagerHelper.SaveFile(config.Path, Path, file);
                    kq.Add("HasError", false);
                    kq.Add("ErrorCode", 0);
                    kq.Add("ErrorHtml", null);
                    kq.Add("Data", true);
                    kq.Add("Error", "");
                }
            }
            catch (Exception ex)
            {
                kq.Add("HasError", true);
                kq.Add("ErrorCode", 2);
                kq.Add("ErrorHtml", null);
                kq.Add("Data", false);
                kq.Add("Error", ex.Message);
            }
            return (kq);
        }
        //[HttpPost]
        public JObject NewFolder(int ID, string path, string name)
        {
            JObject kq = new JObject();
            try
            {
                var user = umbraco.BusinessLogic.User.GetCurrent();
                if (user == null)
                {
                    kq.Add("HasError", true);
                    kq.Add("ErrorCode", 1);
                    kq.Add("ErrorHtml", null);
                    kq.Add("Data", "/umbraco/");
                    kq.Add("Error", "User have to logged in");
                    //kq.AddErrors("", umbraco.library.GetDictionaryItem("Error_Login_TimeOut"));
                    //kq.ErrorCode = 1;
                    //kq.Data = "/umbraco/";
                    return kq;
                }
                var config = FileManagerHelper.GetFileManagerConfig(ID);
                if (config == null)
                {
                    kq.Add("HasError", true);
                    kq.Add("ErrorCode", 2);
                    kq.Add("ErrorHtml", "Cannot found");
                    kq.Add("Data", false);
                    kq.Add("Error", "Cannot found");

                }
                else
                {
                    FileManagerHelper.NewFolder(config.Path, path, name);
                    kq.Add("HasError", false);
                    kq.Add("ErrorCode", 0);
                    kq.Add("ErrorHtml", null);
                    kq.Add("Data", true);
                    kq.Add("Error", "");
                }
            }
            catch (Exception ex)
            {
                kq.Add("HasError", true);
                kq.Add("ErrorCode", 2);
                kq.Add("ErrorHtml", null);
                kq.Add("Data", false);
                kq.Add("Error", ex.Message);
               
            }
            return (kq);
        }

    }
}