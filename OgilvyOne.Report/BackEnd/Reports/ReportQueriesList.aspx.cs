using OgilvyOne.Report.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;

namespace OgilvyOne.Backend.Reports
{
	public partial class ReportQueriesList : umbraco.BasePages.UmbracoEnsuredPage
	{
		public ReportProvider db;
		public ReportQueriesList()
		{
			db = new ReportProvider();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			ImageButton saveButtonData;
			saveButtonData = UmbracoPanel1.Menu.NewImageButton();
			saveButtonData.ID = "btn_Create";
			saveButtonData.Click += new ImageClickEventHandler(btn_Create_Click);
			saveButtonData.AlternateText = "Create";
			saveButtonData.ImageUrl = GlobalSettings.Path + "/images/editor/add.png";

			UmbracoPanel1.Menu.Controls.Add(saveButtonData);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				int total = 0;
				rp_list.DataSource = db.SelectReportQuery(new ReportQueryCondition(), 0, int.MaxValue, out total);
				rp_list.DataBind();
			}
		}

		protected void btn_Create_Click(object sender, ImageClickEventArgs e)
		{
			Response.Redirect("ReportQueryAdd.aspx");
		}

		protected void rp_list_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				ReportQuery mem = (ReportQuery)e.Item.DataItem;
				Button _buttonStatus = (Button)e.Item.FindControl("btn_changestatus");
				if (mem.Status == ReportQueryStatus.Approve)
					_buttonStatus.Text = "UnApprove";
				else
					_buttonStatus.Text = "Approve";
			}
		}

		protected void rp_list_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "ChangeStatus")
			{
				long queryID = Convert.ToInt64(e.CommandArgument);
				var query = db.SelectReportQueryByID(queryID);
				if (query.Status == ReportQueryStatus.UnApprove)
					db.UpdateReportQueryStatus(queryID, ReportQueryStatus.Approve);
				else
					db.UpdateReportQueryStatus(queryID, ReportQueryStatus.UnApprove);

				Response.Redirect(Request.RawUrl);
			}
		}
	}
}