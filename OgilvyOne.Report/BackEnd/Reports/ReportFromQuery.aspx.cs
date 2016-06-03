using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.DataLayer;
using ClientDependency.Core;
using System.Xml;
using OgilvyOne.Report.Model.Reports;
using OgilvyOne.Report.Model.Library;
using OgilvyOne.Report.DataHelper;


namespace OgilvyOne.Backend.Reports
{
	[ClientDependency(ClientDependencyType.Css, "DateTimePicker/datetimepicker.css", "UmbracoClient")]
	[ClientDependency(ClientDependencyType.Javascript, "ui/jquery.js", "UmbracoClient")]
	[ClientDependency(ClientDependencyType.Javascript, "MaskedInput/jquery.maskedinput-1.3.min.js", "UmbracoClient")]
	[ClientDependency(ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
	[ClientDependency(ClientDependencyType.Javascript, "DateTimePicker/timepicker.js", "UmbracoClient")]
	[ClientDependency(ClientDependencyType.Javascript, "DateTimePicker/umbDateTimePicker.js", "UmbracoClient")]
	public partial class ReportFromQuery : umbraco.BasePages.UmbracoEnsuredPage
	{
		#region Pagination
		private const string CurrentPageQueryString = "p";
		private PaginationProvider _CurrentPage;
		protected PaginationProvider CurrentPage
		{
			get
			{
				if (_CurrentPage == null)
				{
					var p = 0;
					if (int.TryParse(Request.QueryString[CurrentPageQueryString], out p) == false)
					{
						p = 1;
					}
					_CurrentPage = new PaginationProvider(p, 20, GetUrlPage);
				}
				return _CurrentPage;
			}
		}
		private string QueryWithOut;
		private string GetUrlPage(int page)
		{
			string urlQuery = "";
			foreach (string item in Request.Url.Query.Split('&'))
			{
				var query = item;
				if (query.StartsWith("?"))
					query = query.Substring(1);
				if (query.StartsWith("p="))
					continue;
				if (urlQuery.Length == 0)
					urlQuery += query;
				else
					urlQuery += "&" + query;
			}

			string urlPath = Request.Url.AbsolutePath;
			if (string.IsNullOrEmpty(urlQuery))
			{
				return string.Format("{0}?{1}={2}", urlPath, CurrentPageQueryString, page);
			}
			else
			{
				return string.Format("{0}?{1}={2}&{3}", urlPath, CurrentPageQueryString, page, urlQuery);
			}
		}
		private string GetUrlNotPage()
		{
			string urlQuery = "";
			foreach (string item in Request.Url.Query.Split('&'))
			{
				var query = item;
				if (query.StartsWith("?"))
					query = query.Substring(1);
				if (query.StartsWith("p="))
					continue;
				if (urlQuery.Length == 0)
					urlQuery += query;
				else
					urlQuery += "&" + query;
			}

			string urlPath = Request.Url.AbsolutePath;
			if (string.IsNullOrEmpty(urlQuery))
			{
				return string.Format("{0}?{1}=", urlPath, CurrentPageQueryString);
			}
			else
			{
				return string.Format("{0}?{1}&{2}=", urlPath, urlQuery, CurrentPageQueryString);
			}
		}
		protected int Total = 0;
		#endregion

		protected long QueryID = 0;
		protected ReportProvider db;
		public ReportFromQuery()
		{
			db = new ReportProvider();
			
		}

		protected ReportQuery reportQuery;
		protected List<ReportProperty> listReportProperties = new List<ReportProperty>();
		protected void Page_Init(object sender, EventArgs e)
		{
			UmbracoPanel1.Menu.Controls.Add(cmdExport2003);
			UmbracoPanel1.Menu.Controls.Add(cmdExport2007);
			this.QueryID = Convert.ToInt32(Request.QueryString["queryreportid"]);
			if (this.QueryID > 0)
			{
				reportQuery = db.SelectReportQueryByID(this.QueryID);
				listReportProperties = db.SelectReportPropertyByReportQuery(reportQuery.ID);
				UmbracoPanel1.Text = reportQuery.Name;
			}
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Total = 0;
				gv_Data.DataSource = this.getData(CurrentPage.Begin, CurrentPage.End, out Total);
				gv_Data.DataBind();

				CurrentPage.SetTotal(Total);
				paging.CurrentPageQueryString = "p";
				paging.CurrentPage = CurrentPage;
				paging.DataBind();
			}
		}
		protected string ExportName()
		{
			return reportQuery.Name.Replace(' ', '_').ToString();
		}
		protected void cmdExport2007_Click(object sender, ImageClickEventArgs e)
		{
			Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			Response.AppendHeader("Content-Disposition", "attachment; filename=Report_" + ExportName() + ".xlsx");

			ExportHelper.Export(Response.OutputStream, this.getData(0, int.MaxValue, out Total));
			Response.End();
		}
		protected void cmdExport2003_Click(object sender, ImageClickEventArgs e)
		{
			Response.ContentType = "application/vnd.ms-excel";
			Response.AppendHeader("Content-Disposition", "attachment; filename=Report_" + ExportName() + ".xls");

			var data = this.getData(0, int.MaxValue, out Total);
			data.TableName = reportQuery.Name;

			XmlTextWriter xmlWriter = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
			xmlWriter.WriteStartDocument();
			data.WriteXml(xmlWriter);
			Response.End();
		}
		
		#region Front-End
		protected string PropertiesNameUpCaseFirstChar(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return null;

			return string.Format("{0}{1}", name[0].ToString().ToUpper(), name.Substring(1));
		}
		protected string PropertiesClass(string properties)
		{
			switch (properties)
			{
				case "Date":
					return "date";
				case "Select":
					return "select";
				case "Checkbox":
					return "checkbox";
				default:
					return "title";
			}
		}
		protected string PropertiesSelect(string propertiesAlias, string value)
		{
			string temp = Request.QueryString[propertiesAlias];
			if (string.IsNullOrWhiteSpace(temp))
				return null;

			temp = temp.ToLower();
			if (!string.IsNullOrWhiteSpace(temp) && temp.Equals(value.ToLower()))
			{
				return "selected";
			}
			return string.Empty;
		}
		protected string[] PropertiesPerValueSplit(string propertiesPreValue)
		{
			if (string.IsNullOrWhiteSpace(propertiesPreValue))
				return null;

			return propertiesPreValue.Split(new string[] { "," }, StringSplitOptions.None);
		}
		protected string PropertiesSelectText(string propertiesItem)
		{
			if (string.IsNullOrWhiteSpace(propertiesItem))
				return string.Empty;
			if (propertiesItem.IndexOf(':') == -1)
				return propertiesItem;
			else
				return propertiesItem.Split(':')[0].Trim();
		}
		protected string PropertiesSelectValue(string propertiesItem)
		{
			if (string.IsNullOrWhiteSpace(propertiesItem))
				return string.Empty;
			if (propertiesItem.IndexOf(':') == -1)
				return propertiesItem;
			else
				return propertiesItem.Split(':')[1].Trim();
		}
		protected string PropertiesConvertDatetime(string propertiesAlias)
		{
			if (string.IsNullOrWhiteSpace(propertiesAlias))
				return null;
			string temp = Request.QueryString[propertiesAlias];
			if (string.IsNullOrWhiteSpace(temp))
				return null;
			return Convert.ToDateTime(temp).ToString("yyyy-MM-dd");
		}
		#endregion

		#region Back-End
		private DataTable getData(int begin, int end, out int total)
		{
			total = 0;
			if (this.reportQuery == null)
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(reportQuery.QueryCount) || string.IsNullOrWhiteSpace(reportQuery.QueryData))
			{
				return null;
			}
           

            DataTable data = new DataTable();
			OgilvyOne.MSSQL.DbProvider db = new OgilvyOne.MSSQL.MSSQLDbProvider(umbraco.BusinessLogic.Application.SqlHelper.ConnectionString);
			db.ClearParameter();

			// Khai báo parameter
			if (this.listReportProperties != null && this.listReportProperties.Any())
			{
				foreach (var properties in this.listReportProperties)
				{
                    if (properties.DataType == ReportPropertyDataType.Date && string.IsNullOrWhiteSpace(Request.QueryString[properties.Alias]))
                    {
                        db.AddParameter(properties.Alias, null);
                    }
                    else
                    {
                        
                        db.AddParameter(properties.Alias, Request.QueryString[properties.Alias]);
                         
                    }
				}
			}
			db.AddParameter("Begin", begin);
			db.AddParameter("End", end);
            
			total = Convert.ToInt32(db.ExecuteScalar(this.reportQuery.QueryCount, CommandType.Text));
            lblTotalRecord.Text = total.ToString();
			data.Load(db.ExecuteReader(this.reportQuery.QueryData, CommandType.Text));
            
			return data;
		}
		#endregion
	}
}