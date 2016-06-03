using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.BasePages;
using Elmah;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace OOUmbracoElmah
{
    public partial class ElmahDetail : UmbracoEnsuredPage
    {        
        protected Literal Output;

        private ErrorLogEntry _errorEntry;

        public ElmahDetail()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string item = base.Request.QueryString["id"];
            bool length = item.Length != 0;
            if (length)
            {
                ErrorLog @default = ErrorLog.GetDefault(HttpContext.Current);
                this._errorEntry = @default.GetError(item);
                length = this._errorEntry != null;
                if (length)
                {
                    base.Title = string.Format("Error: {0} [{1}]", this._errorEntry.Error.Type, this._errorEntry.Id);
                }
                else
                {
                    base.Response.StatusCode = 404;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter);
            bool flag = htmlTextWriter != null;
            if (flag)
            {
                flag = this._errorEntry == null;
                if (flag)
                {
                    ElmahDetail.RenderNoError(htmlTextWriter);
                }
                else
                {
                    this.RenderError(htmlTextWriter);
                }
                this.Output.Text = stringBuilder.ToString();
                return;
            }
            else
            {
                throw new ArgumentNullException("writer");
            }
        }

        private void RenderCollection(HtmlTextWriter writer, NameValueCollection collection, string id, string title)
        {
            bool count;
            string str;
            if (collection == null)
            {
                count = false;
            }
            else
            {
                count = collection.Count != 0;
            }
            bool length = count;
            if (length)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "table-caption");
                writer.RenderBeginTag(HtmlTextWriterTag.P);
                base.Server.HtmlEncode(title, writer);
                writer.RenderEndTag();
                writer.WriteLine();
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "scroll-view");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                Table table = new Table();
                table.CellSpacing = 0;
                TableRow tableRow = new TableRow();
                TableHeaderCell tableHeaderCell = new TableHeaderCell();
                tableHeaderCell.Wrap = false;
                tableHeaderCell.Text = "Name";
                tableHeaderCell.CssClass = "name-col";
                tableRow.Cells.Add(tableHeaderCell);
                tableHeaderCell = new TableHeaderCell();
                tableHeaderCell.Wrap = false;
                tableHeaderCell.Text = "Value";
                tableHeaderCell.CssClass = "value-col";
                tableRow.Cells.Add(tableHeaderCell);
                table.Rows.Add(tableRow);
                string[] allKeys = collection.AllKeys;
                Array.Sort<string>(allKeys);
                int num = 0;
                while (true)
                {
                    length = num < (int)allKeys.Length;
                    if (!length)
                    {
                        break;
                    }
                    string str1 = allKeys[num];
                    TableRow tableRow1 = new TableRow();
                    TableRow tableRow2 = tableRow1;
                    if (num % 2 == 0)
                    {
                        str = "even-row";
                    }
                    else
                    {
                        str = "odd-row";
                    }
                    tableRow2.CssClass = str;
                    TableCell tableCell = new TableCell();
                    tableCell.Text = base.Server.HtmlEncode(str1);
                    tableCell.CssClass = "key-col";
                    tableRow1.Cells.Add(tableCell);
                    tableCell = new TableCell();
                    tableCell.Text = base.Server.HtmlEncode(collection[str1]);
                    tableCell.CssClass = "value-col";
                    tableRow1.Cells.Add(tableCell);
                    table.Rows.Add(tableRow1);
                    num++;
                }
                table.RenderControl(writer);
                writer.RenderEndTag();
                writer.WriteLine();
                writer.RenderEndTag();
                writer.WriteLine();
            }
        }

        private void RenderError(HtmlTextWriter writer)
        {
            Error error = this._errorEntry.Error;
            //writer.AddAttribute(HtmlTextWriterAttribute.Id, "backbtn");
            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:window.history.back();");
            //writer.RenderBeginTag(HtmlTextWriterTag.A);
            //base.Server.HtmlEncode("Back", writer);
            //writer.RenderEndTag();
            //writer.WriteLine();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "PageTitle");
            writer.RenderBeginTag(HtmlTextWriterTag.H1);
            base.Server.HtmlEncode(error.Message, writer);
            writer.RenderEndTag();
            writer.WriteLine();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "ErrorTitle");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "ErrorType");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            base.Server.HtmlEncode(error.Type, writer);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "ErrorTypeMessageSeparator");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(": ");
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "ErrorMessage");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            base.Server.HtmlEncode(error.Message, writer);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.WriteLine();
            bool length = error.Detail.Length == 0;
            if (!length)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, "ErrorDetail");
                writer.RenderBeginTag(HtmlTextWriterTag.Pre);
                writer.Flush();
                base.Server.HtmlEncode(error.Detail, writer.InnerWriter);
                writer.RenderEndTag();
                writer.WriteLine();
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "ErrorLogTime");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            DateTime time = error.Time;
            time = error.Time;
            base.Server.HtmlEncode(string.Format("Logged on {0} at {1}", time.ToLongDateString(), time.ToLongTimeString()), writer);
            writer.RenderEndTag();
            writer.WriteLine();
            length = error.WebHostHtmlMessage.Length == 0;
            if (!length)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Fieldset);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "original-error");
                writer.RenderBeginTag(HtmlTextWriterTag.Legend);
                writer.Write("Original ASP.NET error page");
                writer.RenderEndTag();
                writer.Write(error.WebHostHtmlMessage);
                writer.RenderEndTag();
            }
            this.RenderCollection(writer, error.ServerVariables, "ServerVariables", "Server Variables");
        }

        private static void RenderNoError(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write("Error not found in log.");
            writer.RenderEndTag();
            writer.WriteLine();
        }
    }
}