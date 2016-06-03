using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.uicontrols;
using umbraco.BusinessLogic;
using OgilvyOne.Report.Model.Reports;

namespace OgilvyOne.Backend.Reports
{
	public partial class ReportQueryAdd : umbraco.BasePages.UmbracoEnsuredPage
	{
		public TabPage dataTab;

		protected void Page_Init(object sender, EventArgs e)
		{
			#region Info Tab
			dataTab = TabViewInfo.NewTabPage("Info");
			dataTab.Controls.Add(pane1);
			SetSaveButtonProperties("btnSaveInfo");
			#endregion
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			
		}

		private void SetSaveButtonProperties(string id)
		{
			ImageButton saveButtonData;
			saveButtonData = dataTab.Menu.NewImageButton();
			saveButtonData.ID = id;
			saveButtonData.Click += new ImageClickEventHandler(SaveButton_Click);
			saveButtonData.AlternateText = "Save";
			saveButtonData.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";
			saveButtonData.ValidationGroup = "query";
		}
		protected void SaveButton_Click(object sender, ImageClickEventArgs e)
		{
			Page.Validate();
			if (Page.IsValid && this.ValidateQuery())
			{
				ReportQuery data = new ReportQuery();
				data.Name = txb_Name.Text.Trim();
				data.QueryCount = txb_QueryCount.Text.Trim();
				data.QueryData = txb_QueryData.Text.Trim();
				data.CreatedBy = data.ModifiedBy = this.getUser().LoginName;

				try
				{
					ReportProvider db = new ReportProvider();
					data = db.InsertReportQuery(data);

					if (data.ID > 0)
						Response.Redirect("ReportQueriesList.aspx");
				}
				catch (Exception ex)
				{
					Log.Add(LogTypes.Error, 0, "Create Report Query: " + ex.ToString());
				}
			}
		}
		private bool ValidateQuery()
		{
			bool checkValidate = true;
			if (string.IsNullOrWhiteSpace(txb_Name.Text))
			{
				this.AddErrorMessage("Query name");
				checkValidate = false;
			}
			if (string.IsNullOrWhiteSpace(txb_QueryData.Text))
			{
				this.AddErrorMessage("Query data");
				checkValidate = false;
			}
			return checkValidate;
		}
		protected void AddErrorMessage(string Error)
		{
			CustomValidator cv = new CustomValidator();
			cv.IsValid = false;
			cv.ErrorMessage = Error;
			cv.ValidationGroup = "query";
			this.Page.Validators.Add(cv);
		}
	}
}