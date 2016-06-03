using OgilvyOne.Backend.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.uicontrols;

namespace Ogilvy.Backend.FileManager
{
	public partial class ConfigPage : umbraco.BasePages.UmbracoEnsuredPage
	{
		public FileManagerConfig Model { get; private set; }
		public MenuImageButton AddButton { get; private set; }
		public MenuImageButton SaveButton { get; private set; }

		protected void Page_PreInit(object sender, EventArgs e)
		{
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			AddButton = this.umbracoPanel.Menu.NewImageButton();
			AddButton.Command += AddButton_Command;
			AddButton.ImageUrl = "~/umbraco/images/editor/Add.png";
			SaveButton = this.umbracoPanel.Menu.NewImageButton();
			SaveButton.Command += SaveButton_Command;
			SaveButton.ImageUrl = "~/umbraco/images/editor/Save.GIF";
			SaveButton.Visible = false;

			cboStatus.Items.Add(new ListItem(FileManagerConfigStatus.UnActive.ToDisplay(), FileManagerConfigStatus.UnActive.ToString()));
			cboStatus.Items.Add(new ListItem(FileManagerConfigStatus.Active.ToDisplay(), FileManagerConfigStatus.Active.ToString()));
		}

		void SaveButton_Command(object sender, CommandEventArgs e)
		{
			var item = new FileManagerConfig();
			item.ID = Convert.ToInt32(txbID.Value);
			item.CreatedDate = DateTime.Now;
			item.Name = txbName.Text;
			item.Path = txbPath.Text;
			var user = this.getUser();
			item.CreatedBy = user.Name;
			item.Status = (FileManagerConfigStatus) Enum.Parse(typeof(FileManagerConfigStatus), cboStatus.SelectedValue);
			item.Maxrecord = Convert.ToInt32(txbMaxrecord.Text);
			if (item.Maxrecord <= 0)
			{
				CustomValidatorMaxrecord.IsValid = false;				
			}
			if (this.IsValid)
			{
				if (item.ID == 0)
				{
					FileManagerHelper.InsertFileManagerConfig(item);
				}
				else
				{
					FileManagerHelper.UpdateFileManagerConfig(item);
				}
				ShowList();
			}
		}
		void AddButton_Command(object sender, CommandEventArgs e)
		{
			Model = new FileManagerConfig();
			Model.Maxrecord = 20;
			ShowForm();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ShowList();
			}
		}

		private void ShowList()
		{
			AddButton.Visible = true;
			SaveButton.Visible = false;
			pnlList.Visible = true;
			pnlForm.Visible = false;
			grdData.DataSource = FileManagerHelper.GetListFileManagerConfig(null);
			grdData.DataBind();
		}
		private void ShowForm()
		{
			AddButton.Visible = false;
			SaveButton.Visible = true;
			pnlList.Visible = false;
			pnlForm.Visible = true;
			cboStatus.SelectedValue = Model.Status.ToString();
			pnlForm.DataBind();
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			var item = FileManagerHelper.GetFileManagerConfig(Convert.ToInt32(e.CommandArgument));
			if (item == null)
			{
				AddErrorMessage("Không tìm thấy với ID " + e.CommandArgument);
				return;
			}
			switch (e.CommandName)
			{
				case "EditItem":
					Model = item;
					ShowForm();
					return;
				case "Active":
					item.Status = FileManagerConfigStatus.Active;
					FileManagerHelper.UpdateFileManagerConfig(item);
					break;
				case "UnActive":
					item.Status = FileManagerConfigStatus.UnActive;
					FileManagerHelper.UpdateFileManagerConfig(item);
					break;
			}
			ShowList();
		}
		protected void AddErrorMessage(string Error, string group = "")
		{
			CustomValidator cv = new CustomValidator();
			cv.IsValid = false;
			cv.ErrorMessage = Error;
			cv.ValidationGroup = group;
			this.Page.Validators.Add(cv);
		}


	}

}