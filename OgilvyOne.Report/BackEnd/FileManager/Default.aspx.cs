using OgilvyOne.Backend.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ogilvy.Backend.FileManager
{
	public partial class Default : umbraco.BasePages.UmbracoEnsuredPage
	{
		[System.ComponentModel.DataObject]
		public class FileManagerDataObject
		{
			public FileManagerConfig Config { get;  private set; }

			public FileManagerDataObject(FileManagerConfig config)
			{
				this.Config = config;
			}
			public int PageSize
			{
				get
				{
					if (Config == null || Config.Maxrecord <= 0)
					{
						return 10;
					}
					return Config.Maxrecord;
				}
			}
			public string Name
			{
				get
				{
					if (Config == null)
					{
						return "";
					}
					return Config.Name;
				}
			}

			[System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
			public List<FileItem> GetListItem(string path)
			{
				if(Config == null)
				{
					 return new List<FileItem>();
				}
				return FileManagerHelper.GetListFile(Config.Path, path);
			}
		}

		private FileManagerDataObject _DataObject;
		public FileManagerDataObject DataObject
		{
			get
			{
				if (_DataObject == null)
				{
					var config = FileManagerHelper.GetFileManagerConfig(Convert.ToInt32(Request.QueryString["ID"]));
					_DataObject = new FileManagerDataObject(config);
				}
				return _DataObject;
			}
		}
		public Default()
		{
			
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			panel1.Menu.Controls.Add(cmdDelete);
			panel1.Menu.Controls.Add(cmdAdd);
			panel1.Menu.Controls.Add(cmdAddFolder);
            panel1.Menu.Controls.Add(cmdDownload);
			panel1.Menu.NewElement("checkbox", "a", "", 10);
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			panel1.Text = "File manager: [" + DataObject.Name + "]/" +Request.QueryString["path"];
			if (!IsPostBack)
			{
				DataPager1.PageSize = DataObject.PageSize;
			}
		}
		protected void Page_LoadComplete(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{

			}
		}

		protected void cmdDelete_Click(object sender, ImageClickEventArgs e)
		{
			if(DataObject.Config == null)
			{
				return;
			}
			foreach (var item in rpData.Items)
			{
				var chk = item.FindControl("chkCheck") as CheckBox;
				if (chk != null && chk.Checked)
				{
					var tbxKey = item.FindControl("tbxKey") as HiddenField;
					FileManagerHelper.Delete(DataObject.Config.Path, tbxKey.Value);
				}
			}
			rpData.DataBind();
		}
        protected void cmdDownload_Click(object sender, ImageClickEventArgs e)
        {
            int totalFileChosen = 0;
            if (DataObject.Config == null)
            {
                return;
            }
            foreach (var item in rpData.Items)
            {
                var chk = item.FindControl("chkCheck") as CheckBox;
                if (chk != null && chk.Checked)
                {
                    totalFileChosen++;
                }
            }
            if (totalFileChosen <= 1)
            {
                foreach (var item in rpData.Items)
                {
                    var chk = item.FindControl("chkCheck") as CheckBox;
                    if (chk != null && chk.Checked)
                    {
                        totalFileChosen++;
                        var tbxKey = item.FindControl("tbxKey") as HiddenField;

                       
                        // get the file attributes for file or directory
                        FileAttributes attr = File.GetAttributes(HttpContext.Current.Server.MapPath(DataObject.Config.Path + tbxKey.Value));

                        if (attr.HasFlag(FileAttributes.Directory)){
                            pnlContent.Text ="Vui lòng chọn file để tải, không thể tải thư mục";
                        }
                        else{
                            Response.Clear();

                            Stream iStream = null;

                            const int bufferSize = 64 * 1024;

                            byte[] buffer = new Byte[bufferSize];
                            int length;
                            long dataToRead;
                            try
                            {
                                string[] fileNamePath = tbxKey.Value.Split('/');
                                string fileName = fileNamePath[fileNamePath.Length - 1];
                                iStream = new FileStream(HttpContext.Current.Server.MapPath(DataObject.Config.Path + tbxKey.Value), FileMode.Open, FileAccess.Read, FileShare.Read);
                                dataToRead = iStream.Length;
                                Response.ContentType = "application/octet-stream";
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                                while (dataToRead > 0)
                                {
                                    if (Response.IsClientConnected)
                                    {
                                        length = iStream.Read(buffer, 0, bufferSize);
                                        Response.OutputStream.Write(buffer, 0, length);
                                        Response.Flush();
                                        buffer = new byte[bufferSize];
                                        dataToRead = dataToRead - length;
                                    }
                                    else
                                    {
                                        //prevent infinate loop on disconnect
                                        dataToRead = -1;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //Your exception handling here
                            }
                            finally
                            {
                                if (iStream != null)
                                {
                                    iStream.Close();
                                }
                                Response.Close();
                            }
                        }
                        
                    }
                }
                rpData.DataBind();
            }
            else {
                 pnlContent.Text = "Hệ thống chỉ cho phép tải 1 file mỗi lượt";
            }
        }

		protected void AddErrorMessage(string Error,string group = "")
		{
			CustomValidator cv = new CustomValidator();
			cv.IsValid = false;
			cv.ErrorMessage = Error;
			cv.ValidationGroup = group;
			this.Page.Validators.Add(cv);
		}
		protected void ObjectDataSource1_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddErrorMessage(e.Exception.Message);
                Utils.Log("Ogilvy.Backend.FileManager.Default: ", e.Exception);
				e.ExceptionHandled = true;
			}
		}

		protected void ObjectDataSource1_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
		{
			e.ObjectInstance = DataObject;			
		}

	}
}