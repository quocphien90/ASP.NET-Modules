using OgilvyOne.Report.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace OgilvyOne.Report.Backend.controls
{
	public partial class PaginationControl : System.Web.UI.UserControl
	{
		public string CurrentPageQueryString { get; set; }
		public PaginationProvider CurrentPage { get; set; }

		protected string pagingState = "";
		protected int pagingBlockStart = 0;
		protected int pagingBlockEnd = 0;
		protected string GetPaginState(out int middleBlockStart, out int middleBlockFinish)
		{
			// Middle block limits
			middleBlockStart = 0;
			middleBlockFinish = 0;
			var middleBlockCount = 4;

			int pageNumber = CurrentPage.CurrentPage; // Get information from query string
			int numberOfPages = CurrentPage.TotalPage; // Get the total number of pages

			// There are three possible paging states: left, middle and right
			// First we determine what state the paging is at and 
			// set the middle block limits
			string pagingState = string.Empty;
			if (CurrentPage.TotalPage <= 1)
				return pagingState;

			if (pageNumber >= middleBlockCount && pageNumber <= (numberOfPages - middleBlockCount + 1))
			{
				pagingState = "middle";
				middleBlockStart = pageNumber - (int)(middleBlockCount / 2);
			}
			else if (pageNumber < middleBlockCount && numberOfPages < middleBlockCount)
			{
				pagingState = "left-limit";
				middleBlockStart = 1;
			}
			else if (pageNumber < middleBlockCount)
			{
				pagingState = "left";
				middleBlockStart = 1;
			}
			else
			{
				pagingState = "right";
				middleBlockStart = numberOfPages - middleBlockCount;
			}

			middleBlockFinish = middleBlockStart + middleBlockCount;

			//return
			return pagingState;
		}
		protected string GetUrlNotPage()
		{
			string urlQuery = "";
			foreach (string item in Request.Url.Query.Split('&'))
			{
				var query = item;
				if (query.StartsWith("?"))
					query = query.Substring(1);
				if (query.StartsWith(CurrentPageQueryString + "="))
					continue;
				if (urlQuery.Length == 0)
					urlQuery += query;
				else
					urlQuery += "&" + query;
			}

			string urlPath = Request.Url.PathAndQuery.Split('?')[0];
			if (string.IsNullOrEmpty(urlQuery))
			{
				return string.Format("{0}?{1}=", urlPath, CurrentPageQueryString);
			}
			else
			{
				return string.Format("{0}?{1}&{2}=", urlPath, urlQuery, CurrentPageQueryString);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Pagination_DataBind();
			}
		}

		public void Pagination_DataBind()
		{
			if (CurrentPage != null && CurrentPage.TotalPage > 0
				&& !string.IsNullOrWhiteSpace(CurrentPageQueryString))
			{
				this.pagingState = this.GetPaginState(out pagingBlockStart, out pagingBlockEnd);
				rpPage.DataSource = CurrentPage;
				rpPage.DataBind();
			}
		}

		protected void rpPage_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				if (this.pagingState == "middle" || this.pagingState == "right")
				{
					PaginationItem pagination = CurrentPage.First();
					HyperLink _name = (HyperLink)e.Item.FindControl("pageLink");

					if (pagination.Page != CurrentPage.CurrentPage)
					{
						_name.NavigateUrl = this.ResolveUrl(pagination.Url);
					}
					_name.Text = "First";
				}
				else
				{
					e.Item.Visible = false;
				}
			}

			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				PaginationItem pagination = (PaginationItem)e.Item.DataItem;
				if (pagination.Page >= this.pagingBlockStart && pagination.Page <= this.pagingBlockEnd)
				{
					HyperLink _name = (HyperLink)e.Item.FindControl("pageLink");
					if (pagination.Page != CurrentPage.CurrentPage)
					{
						_name.NavigateUrl = this.ResolveUrl(pagination.Url);
					}
					_name.Text = pagination.Page.ToString();
				}
				else
				{
					e.Item.Visible = false;
				}
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				if (this.pagingState == "middle" || this.pagingState == "left")
				{
					PaginationItem pagination = CurrentPage.Last();
					HyperLink _name = (HyperLink)e.Item.FindControl("pageLink");

					if (pagination.Page != CurrentPage.CurrentPage)
					{
						_name.NavigateUrl = this.ResolveUrl(pagination.Url);
					}
					_name.Text = "Last";
				}
				else
				{
					e.Item.Visible = false;
				}
			}
		}
	}
}