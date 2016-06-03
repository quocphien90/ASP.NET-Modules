using OgilvyOne.Report.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.uicontrols;


namespace OgilvyOne.Backend.Reports
{
	public partial class ReportQueryEdit : umbraco.BasePages.UmbracoEnsuredPage
	{
		public TabPage dataTab;
		public long QueryID = 0;
		
		public ReportProvider db;
		public ReportQueryEdit()
		{
			db = new ReportProvider();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			#region Query Tab
			dataTab = TabViewInfo.NewTabPage("Info");
			dataTab.Controls.Add(pane1);
			SetSaveButtonProperties("btnSaveInfo", "");
			#endregion

			#region Properties Tab
			dataTab = TabViewInfo.NewTabPage("Properties");
			dataTab.Controls.Add(panePropertiesCreateEdit);
			dataTab.Controls.Add(panePropertiesView);
			SetSaveButtonProperties("btnSavePro", "return dataOrder();");
			#endregion
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			long.TryParse(Request.QueryString["id"], out QueryID);
			
			if (!IsPostBack)
			{
				#region dropdownlist Status
				Array itemStatusValues = System.Enum.GetValues(typeof(ReportQueryStatus));
				Array itemStatusNames = System.Enum.GetNames(typeof(ReportQueryStatus));

				for (int i = 0; i <= itemStatusNames.Length - 1; i++)
				{
					ddl_Status.Items.Add(new ListItem(itemStatusNames.GetValue(i).ToString(), itemStatusValues.GetValue(i).ToString()));
				}
				#endregion

				#region dropdownlist Properties DataType
				ddl_PropertiesDataType.Items.Add(new ListItem() { Text = "All", Value = "" });

				Array itemValues = System.Enum.GetValues(typeof(ReportPropertyDataType));
				Array itemNames = System.Enum.GetNames(typeof(ReportPropertyDataType));

				for (int i = 0; i <= itemNames.Length - 1; i++)
				{
					ListItem item = new ListItem(itemNames.GetValue(i).ToString(), itemValues.GetValue(i).ToString());
					ddl_PropertiesDataType.Items.Add(item);
				}
				#endregion

				if (QueryID <= 0)
				{
					Response.Redirect("ReportQueriesList.aspx");
				}
				else
				{
					this.ReloadData();
				}
			}
		}

		protected void ReloadData()
		{
			var data = db.SelectReportQueryByID(QueryID);
			txb_Name.Text = data.Name;
			txb_QueryCount.Text = data.QueryCount;
			txb_QueryData.Text = data.QueryData;

			if (data.Status == ReportQueryStatus.UnApprove)
				ddl_Status.SelectedIndex = 0;
			else
				ddl_Status.SelectedIndex = 1;

			this.ReloadPanelProperties();
		}

		#region Query
		protected void SetSaveButtonProperties(string id, string onClientClick)
		{
			ImageButton saveButtonData;
			saveButtonData = dataTab.Menu.NewImageButton();
			saveButtonData.ID = id;
			saveButtonData.Click += new ImageClickEventHandler(SaveButton_Click);
			saveButtonData.AlternateText = "Save";
			saveButtonData.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";
			saveButtonData.ValidationGroup = "query";
			saveButtonData.OnClientClick = onClientClick;
		}

		protected void SaveButton_Click(object sender, ImageClickEventArgs e)
		{
			Page.Validate("query");
			if (Page.IsValid && this.ValidateQuery())
			{
				var data = db.SelectReportQueryByID(QueryID);
				data.Name = txb_Name.Text;
				data.QueryCount = txb_QueryCount.Text;
				data.QueryData = txb_QueryData.Text;
				data.ModifiedDate = DateTime.Now;
				data.ModifiedBy = this.getUser().LoginName;
				data.Status = (ReportQueryStatus)Enum.Parse(typeof(ReportQueryStatus), ddl_Status.SelectedValue);

				try
				{
					db.UpdateReportQuery(data);
					//Save properties sort order
					if (!string.IsNullOrWhiteSpace(hd_listID.Value))
					{
						int[] listItem = new int[db.SelectReportPropertyByReportQuery(this.QueryID).Count];
						string[] listID = hd_listID.Value.Split(',');
						if (listItem.Length == listID.Length)
						{
							for (int i = 0; i < listID.Length; i++)
							{
								listItem[i] = Convert.ToInt32(listID[i]);
							}
						}
						if (listItem.Any())
						{
							db.ChangeSortOrderPropertiy(this.QueryID, listItem);
						}
					}

					this.ReloadData();
					ClientTools.ShowSpeechBubble(speechBubbleIcon.success, "Save query successful", "");
				}
				catch (Exception ex)
				{
					Log.Add(LogTypes.Error, 0, "Edit Report Query: " + ex.ToString());
				}
			}
		}
		protected bool ValidateQuery()
		{
			bool checkValidate = true;
			if (string.IsNullOrWhiteSpace(txb_Name.Text))
			{
				this.AddErrorMessage("Query name", "query");
				checkValidate = false;
			}
			if (string.IsNullOrWhiteSpace(txb_QueryData.Text))
			{
				this.AddErrorMessage("Query data", "query");
				checkValidate = false;
			}
			return checkValidate;
		}
		protected void AddErrorMessage(string Error, string Group)
		{
			CustomValidator cv = new CustomValidator();
			cv.IsValid = false;
			cv.ErrorMessage = Error;
			cv.ValidationGroup = Group;
			this.Page.Validators.Add(cv);
		}
		#endregion

		#region Properties
		protected void btn_PropertiesAdd_Click(object sender, EventArgs e)
		{
			btn_PropertiesAdd.Visible = false;
			panePropertiesInfo.Visible = true;
			panePropertiesInfo.Text = "Add properties";
			PropertiesClearValue();
		}

		protected void btn_PropertiesCancel_Click(object sender, EventArgs e)
		{
			btn_PropertiesAdd.Visible = true;
			panePropertiesInfo.Visible = false;
			PropertiesClearValue();
		}

		protected void ReloadPanelProperties()
		{
			rp_Propertise.DataSource = db.SelectReportPropertyByReportQuery(this.QueryID);
			rp_Propertise.DataBind();

			btn_PropertiesAdd.Visible = true;
			panePropertiesInfo.Visible = false;
			PropertiesClearValue();
		}
		private void PropertiesClearValue()
		{
			hd_PropertiesID.Value = string.Empty;
			txb_PropertiesAlias.Text = string.Empty;
			txb_PropertiesName.Text = string.Empty;
			ddl_PropertiesDataType.SelectedIndex = 0;
			cb_PropertiesMandatory.Checked = false;
			txb_PropertiesPreValue.Text = string.Empty;
			txb_PropertiesDescription.Text = string.Empty;
		}

		protected void btn_PropertiesSave_Click(object sender, EventArgs e)
		{
			try
			{
				Page.Validate("properties");
				if (Page.IsValid && ValidateProperties())
				{
					ReportProperty propertiesData = new ReportProperty();

					long proID = 0;
					if (!string.IsNullOrWhiteSpace(hd_PropertiesID.Value))
					{
						long.TryParse(hd_PropertiesID.Value, out proID);
					}

					//Input dữ liệu
					propertiesData.QueryID = this.QueryID;
					propertiesData.Alias = txb_PropertiesAlias.Text;
					propertiesData.Name = txb_PropertiesName.Text;
					propertiesData.DataType = (ReportPropertyDataType)Enum.Parse(typeof(ReportPropertyDataType), ddl_PropertiesDataType.SelectedValue);
					propertiesData.Mandatory = cb_PropertiesMandatory.Checked;
					propertiesData.DataPreValue = txb_PropertiesPreValue.Text;
					propertiesData.Description = txb_PropertiesDescription.Text;


					//Insert mới properties
					if (proID == 0)
					{
						db.InsertReportProperty(propertiesData);
					}
					//Update properties
					else
					{
						propertiesData.ID = proID;
						db.UpdateReportProperty(propertiesData);
					}
					this.ReloadData();
				}
			}
			catch (Exception ex)
			{
				Log.Add(LogTypes.Error, 0, "Save Report Properties: " + ex.ToString());
			}
		}

		protected bool ValidateProperties()
		{
			bool checkValidate = true;
			if (string.IsNullOrWhiteSpace(txb_PropertiesName.Text))
			{
				this.AddErrorMessage("Name is required", "properties");
				checkValidate = false;
			}
			if (string.IsNullOrWhiteSpace(txb_PropertiesAlias.Text))
			{
				this.AddErrorMessage("Alias is required", "properties");
				checkValidate = false;
			}
			if (txb_PropertiesAlias.Text.IndexOf(' ') != -1)
			{
				this.AddErrorMessage("Alias is not has space", "properties");
				checkValidate = false;
			}
			if (txb_PropertiesAlias.Text.ToLower().Equals("queryreportid"))
			{
				this.AddErrorMessage("Alias 'queryreportid' is properties system", "properties");
				checkValidate = false;
			}
			if (txb_PropertiesAlias.Text.IndexOf(' ') > -1)
			{
				this.AddErrorMessage("Alias not has white space", "properties");
				checkValidate = false;
			}
			if (string.IsNullOrWhiteSpace(ddl_PropertiesDataType.SelectedValue))
			{
				this.AddErrorMessage("Data Type is required", "properties");
				checkValidate = false;
			}
			return checkValidate;
		}

		protected void rp_Propertise_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				btn_PropertiesAdd.Visible = false;
				panePropertiesInfo.Visible = true;
				panePropertiesInfo.Text = "Update properties";
				PropertiesClearValue();

				long propertiesID = Convert.ToInt64(e.CommandArgument);

				var propertiesData = db.SelectReportProperty(propertiesID);
				hd_PropertiesID.Value = propertiesData.ID.ToString();
				txb_PropertiesAlias.Text = propertiesData.Alias;
				txb_PropertiesName.Text = propertiesData.Name;
				ddl_PropertiesDataType.SelectedIndex = ((int)propertiesData.DataType) + 1;
				cb_PropertiesMandatory.Checked = propertiesData.Mandatory;
				txb_PropertiesPreValue.Text = propertiesData.DataPreValue;
				txb_PropertiesDescription.Text = propertiesData.Description;
			}
			if (e.CommandName == "Remove")
			{
				long propertiesID = Convert.ToInt64(e.CommandArgument);
				db.DeleteReportProperty(propertiesID);
				this.ReloadData();
			}
		}

		#endregion

	}
}