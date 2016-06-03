using OgilvyOne.Report.DataHelper;
using OgilvyOne.Report.Model.Library;
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

namespace Huggies.CoreUI.Backend.Reports
{
	public partial class ReportActiveMemberScheme : umbraco.BasePages.UmbracoEnsuredPage
	{
		public class Condition
		{
			public DateTime? FromDate { get; set; }
			public DateTime? ToDate { get; set; }
			public string MemberType { get; set; }
		}
		public Condition _condition;
		public Condition GetCondition()
		{
			if (_condition == null)
			{
				_condition = new Condition();
				if (Request.QueryString["f_fromdate"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["f_fromdate"]))
				{
					_condition.FromDate = f_fromdate.DateTime = Convert.ToDateTime(Request.QueryString["f_fromdate"]);
				}
				if (Request.QueryString["f_todate"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["f_todate"]))
				{
					_condition.ToDate = f_todate.DateTime = Convert.ToDateTime(Request.QueryString["f_todate"]);
				}
				_condition.MemberType = Request.QueryString["f_membertype"];
			}
			return _condition;
		}
		protected string selectValue(string value, string querystring)
		{
			string queryvalue = Request.QueryString[querystring];
			if (value.Equals(queryvalue))
			{
				return "selected=\"selected\"";
			}
			return string.Empty;
		}


		#region Pagination
		public const string CurrentPageQueryString = "p";
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
			if (QueryWithOut == null)
			{
				QueryWithOut = Request.GetQueryWithOut(CurrentPageQueryString);
			}
			var sep = QueryWithOut.Length > 0 ? "&" : "";
			return this.ResolveUrl(string.Format("ReportActiveMemberScheme.aspx?{0}{1}{2}={3}", QueryWithOut, sep, CurrentPageQueryString, page));
		}
		protected int Total = 0;
		protected string SqlExecute = string.Empty;
		#endregion

		protected void Page_Init(object sender, EventArgs e)
		{
			UmbracoPanel1.Menu.Controls.Add(cmdExport);

		}
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Total = 0;
				gv_Data.DataSource = getData(GetCondition() ,CurrentPage.Begin, CurrentPage.End, out Total);
				gv_Data.DataBind();

				CurrentPage.SetTotal(Total);
				paging.CurrentPageQueryString = CurrentPageQueryString;
				paging.CurrentPage = this.CurrentPage;
				paging.Pagination_DataBind();
			}
		}
		
		protected void cmdExport_Click(object sender, ImageClickEventArgs e)
		{
			Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			Response.AppendHeader("Content-Disposition", "attachment; filename=ReportData.xlsx");

			ExportHelper.Export(Response.OutputStream, getData(GetCondition(), 0, int.MaxValue, out Total));
			Response.End();
		}

		private DataTable getData(Condition condition, int begin, int end, out int total)
		{
			total = 0;

			List<IParameter> listParameter = new List<IParameter>();
			listParameter.Add(SqlHelper.CreateParameter("FromDate", condition.FromDate));
            listParameter.Add(SqlHelper.CreateParameter("ToDate", condition.ToDate));
            listParameter.Add(SqlHelper.CreateParameter("MemberType", condition.MemberType));

			total = SqlHelper.ExecuteScalar<int>(@"DECLARE @Logs TABLE(MemberID bigint, Logs int)
			INSERT INTO @Logs
			SELECT MemberID, COUNT(1) [Logs] FROM memHuggiesMemberLogs 
			WHERE CatID = 2 
				AND	( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [CreatedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [CreatedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [CreatedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY MemberID
			HAVING COUNT(1) > 0

			DECLARE @Experts TABLE(MemberID bigint, [Public] int, [UnPublic] int)
			INSERT INTO @Experts
			SELECT MemberID, SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END)[Public],SUM(CASE WHEN IsPublished <> 1 THEN 1 ELSE 0 END)[UnPublic]  
				FROM epQuestions 
			WHERE  ( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [AddedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [AddedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [AddedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY MemberID

			DECLARE @ForumPosts TABLE(MemberID bigint, [Public] int, [UnPublic] int)
			INSERT INTO @ForumPosts
			SELECT AddedById, SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END)[Public],SUM(CASE WHEN IsPublished <> 1 THEN 1 ELSE 0 END)[UnPublic] FROM frPosts 
			WHERE IsDeleted = 0 
				AND ( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [AddedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [AddedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [AddedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY AddedById

			DECLARE @ForumComments TABLE(MemberID bigint, [Public] int, [UnPublic] int)
			INSERT INTO @ForumComments
			SELECT AddedById, SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END)[Public],SUM(CASE WHEN IsPublished <> 1 THEN 1 ELSE 0 END)[UnPublic] FROM frComments 
			WHERE IsDeleted = 0 AND IsBadWordList = 0 
				AND ( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [AddedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [AddedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [AddedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY AddedById


			SELECT COUNT(1)[Total]
			FROM memHuggiesMembers AS [m]
				LEFT JOIN @Logs AS [l] ON m.ID = l.MemberID
				LEFT JOIN @Experts AS [e] ON m.ID = e.MemberID
				LEFT JOIN @ForumPosts AS [p] ON m.ID = p.MemberID
				LEFT JOIN @ForumComments AS [c] ON m.ID = c.MemberID
				LEFT JOIN cmsMember2MemberGroup AS [g] ON m.ID = g.Member
			WHERE [m].[Status] in (101,103) 
				AND (LEN(@MemberType) = 0 OR @MemberType IS NULL 
						OR (@MemberType = 'Expert' AND m.IsExpert = 1)
						OR (@MemberType = 'Mod' AND m.IsExpert = 0 AND g.Member IS NOT NULL)
						OR (@MemberType = 'Member' AND m.IsExpert = 0 AND g.Member IS NULL)
					)
				AND (l.Logs > 0 OR e.[Public] > 0 OR e.[UnPublic] > 0 OR p.[Public] > 0 OR p.[UnPublic] > 0 OR c.[Public] > 0 OR c.[UnPublic] > 0)
			", listParameter.ToArray());
			

			SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString);
			SqlCommand command = new SqlCommand();
			DataTable data = new DataTable();

			command.Connection = conn;
			command.CommandText = @"DECLARE @Logs TABLE(MemberID bigint, Logs int)
			INSERT INTO @Logs
			SELECT MemberID, COUNT(1) [Logs] FROM memHuggiesMemberLogs 
			WHERE CatID = 2 
				AND	( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [CreatedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [CreatedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [CreatedDate] BETWEEN @FromDate AND @ToDate)
					) 
			GROUP BY MemberID
			HAVING COUNT(1) > 0

			DECLARE @Experts TABLE(MemberID bigint, [Public] int, [UnPublic] int)
			INSERT INTO @Experts
			SELECT MemberID, SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END)[Public],SUM(CASE WHEN IsPublished <> 1 THEN 1 ELSE 0 END)[UnPublic]  
				FROM epQuestions 
			WHERE  ( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [AddedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [AddedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [AddedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY MemberID

			DECLARE @ForumPosts TABLE(MemberID bigint, [Public] int, [UnPublic] int)
			INSERT INTO @ForumPosts
			SELECT AddedById, SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END)[Public],SUM(CASE WHEN IsPublished <> 1 THEN 1 ELSE 0 END)[UnPublic] FROM frPosts 
			WHERE IsDeleted = 0 
				AND ( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [AddedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [AddedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [AddedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY AddedById

			DECLARE @ForumComments TABLE(MemberID bigint, [Public] int, [UnPublic] int)
			INSERT INTO @ForumComments
			SELECT AddedById, SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END)[Public],SUM(CASE WHEN IsPublished <> 1 THEN 1 ELSE 0 END)[UnPublic] FROM frComments 
			WHERE IsDeleted = 0 AND IsBadWordList = 0 
				AND ( (@FromDate IS NULL AND @ToDate IS NULL) 
						OR
						(@FromDate IS NOT NULL AND @ToDate IS NULL AND [AddedDate] >= @FromDate)
						OR
						(@FromDate IS NULL AND @ToDate IS NOT NULL AND [AddedDate] <= @ToDate)
						OR 
						(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND [AddedDate] BETWEEN @FromDate AND @ToDate)
					)
			GROUP BY AddedById

			SELECT *
				FROM (
					SELECT ROW_NUMBER() OVER(ORDER BY m.ID) AS [RowNum]
							, m.ID, m.Email, CONVERT(NVARCHAR,m.[Status])[Status], CASE WHEN m.IsExpert = 1 THEN 'Expert' WHEN g.Member IS NOT NULL THEN 'Moderator' ELSE 'Member' END [MemberType]
							, ISNULL(l.Logs,0)[Login]
							, ISNULL(e.[Public],0)[ExpertPublic], ISNULL(e.[UnPublic],0)[ExpertUnPublic]
							, ISNULL(p.[Public],0)[ForumsPostPublic], ISNULL(p.[UnPublic],0)[ForumsPostUnPublic]
							, ISNULL(c.[Public],0)[ForumsCommentPublic], ISNULL(c.[UnPublic],0)[ForumsCommentUnPublic]
						FROM memHuggiesMembers AS [m]
							LEFT JOIN @Logs AS [l] ON m.ID = l.MemberID
							LEFT JOIN @Experts AS [e] ON m.ID = e.MemberID
							LEFT JOIN @ForumPosts AS [p] ON m.ID = p.MemberID
							LEFT JOIN @ForumComments AS [c] ON m.ID = c.MemberID
							LEFT JOIN cmsMember2MemberGroup AS [g] ON m.ID = g.Member
					WHERE [m].[Status] in (101,103) 
						AND (LEN(@MemberType) = 0 OR @MemberType IS NULL 
								OR (@MemberType = 'Expert' AND m.IsExpert = 1)
								OR (@MemberType = 'Moderator' AND m.IsExpert = 0 AND g.Member IS NOT NULL)
								OR (@MemberType = 'Member' AND m.IsExpert = 0 AND g.Member IS NULL)
							)
						AND (l.Logs > 0 OR e.[Public] > 0 OR e.[UnPublic] > 0 OR p.[Public] > 0 OR p.[UnPublic] > 0 OR c.[Public] > 0 OR c.[UnPublic] > 0)
				) AS [T]
			WHERE [T].[RowNum] BETWEEN @Begin AND @End
			";
			if(condition.FromDate == null)
				command.Parameters.Add(new SqlParameter("FromDate", DBNull.Value)); 
			else
				command.Parameters.Add(new SqlParameter("FromDate", condition.FromDate)); 
			if (condition.ToDate == null)
				command.Parameters.Add(new SqlParameter("ToDate", DBNull.Value)); 
			else
				command.Parameters.Add(new SqlParameter("ToDate", condition.ToDate));
			if (condition.MemberType == null)
				command.Parameters.Add(new SqlParameter("MemberType", DBNull.Value)); 
			else
				command.Parameters.Add(new SqlParameter("MemberType", condition.MemberType));
			command.Parameters.Add(new SqlParameter("Begin", begin));
			command.Parameters.Add(new SqlParameter("End", end));

			try
			{
				conn.Open();
				data.Load(command.ExecuteReader());
			}
			finally
			{
				conn.Close();
			}

			data.Columns["Status"].ReadOnly = false;
			foreach (DataRow row in data.Rows)
			{
				try
				{
					row["Status"] = ((HuggiesMemberStatus)Convert.ToInt32(row["Status"])).ToDisplay();
				}
				catch { }
			}

			return data;
		}

		protected void gv_Data_DataBound(object sender, EventArgs e)
		{
			
		}
	}
}