<%@ Page Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="ReportFromQuery.aspx.cs" Inherits="OgilvyOne.Backend.Reports.ReportFromQuery" %>

<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols.DatePicker" Assembly="controls" %>
<%@ Register TagPrefix="pagination" TagName="Page" Src="../controls/PaginationControl.ascx" %>
<%@ Import Namespace="OgilvyOne.Report.Model.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link type="text/css" href="/css/backend.css" rel="stylesheet" />
    <script type="text/javascript" src="/scripts/cms.backend.core.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <cc1:UmbracoPanel ID="UmbracoPanel1" runat="server" Text="Report data" hasMenu="true">
        <cc1:MenuImageButton runat="server" ID="cmdExport2007" ImageUrl="~/umbraco/images/exportDocumenttype.png" ToolTip="Export Excel 2007" AltText="Export" OnClick="cmdExport2007_Click" />
        <cc1:MenuImageButton runat="server" ID="cmdExport2003" ImageUrl="~/umbraco/images/exportDocumenttype.png" ToolTip="Export Excel 2003" AltText="Export" OnClick="cmdExport2003_Click" />
        <cc1:Pane ID="pane1" runat="server">
           <div style="margin-left:5px;"> <b>Total : <asp:Label ID="lblTotalRecord" ForeColor="Red" runat="server" Text=""></asp:Label></b></div><br />
            <% if (this.listReportProperties.Any())
               { %>
            <div class="search" og-find="true">
                <% foreach (var properties in this.listReportProperties)
                   { %>
                <div class="<%= this.PropertiesClass(properties.DataType.ToDisplay()) %>">
                    <%= this.PropertiesNameUpCaseFirstChar(properties.Name) %><br />
                    <% if (properties.DataType == ReportPropertyDataType.String || properties.DataType == ReportPropertyDataType.Int)
                       { %>
                    <input type="text" name="<%= properties.Alias %>" value="<%= Request.QueryString[properties.Alias] %>" />
                    <% }
                       else if (properties.DataType == ReportPropertyDataType.Checkbox)
                       { %>
                    <input type="checkbox" name="<%= properties.Alias %>" value="<%= Request.QueryString[properties.Alias] %>" />
                    <% }
                       else if (properties.DataType == ReportPropertyDataType.Select)
                       { %>
                    <select name="<%= properties.Alias %>">
                        <option value="">--select--</option>
                        <% foreach (string item in this.PropertiesPerValueSplit(properties.DataPreValue))
                           { %>
                                <option value="<%=this.PropertiesSelectValue(item)%>" <%= this.PropertiesSelect(properties.Alias, this.PropertiesSelectValue(item)) %>><%=this.PropertiesSelectText(item)%></option>
                        <% } %>
                    </select>
                    <% }
                       else if (properties.DataType == ReportPropertyDataType.Date)
                       { %>
                    <div class="umbDateTimePicker">
                        <input type="text" id="<%= properties.Alias %>" name="<%= properties.Alias %>" value="<%= this.PropertiesConvertDatetime(properties.Alias) %>" />
                        <% if (string.IsNullOrWhiteSpace(this.PropertiesConvertDatetime(properties.Alias))) { %>
                            <div>No date chosen</div>
                        <% } else { %>
                            <a>Clear Date</a>
                        <% } %>
                    </div>
                    <% } %>
                </div>
                <% } %>
                <div class="button">
                    <input type="button" value="Search" og-find-command="true" href="<%= string.Format("{0}?queryreportid={1}",Request.Url.AbsolutePath,this.QueryID) %>" />
                </div>
            </div>
            <% } %>
        </cc1:Pane>
        <asp:Panel ID="panel" runat="server">
            <div data="scroll" style="width: 100%; overflow-x: scroll;">
                <asp:GridView ID="gv_Data" runat="server" CellPadding="4" PagerSettings-PageButtonCount="20" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>

                    <EditRowStyle BackColor="#2461BF"></EditRowStyle>

                    <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True"></FooterStyle>

                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"></HeaderStyle>

                    <PagerStyle HorizontalAlign="Center" BackColor="#2461BF" ForeColor="White"></PagerStyle>

                    <RowStyle BackColor="#EFF3FB"></RowStyle>

                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333"></SelectedRowStyle>

                    <SortedAscendingCellStyle BackColor="#F5F7FB"></SortedAscendingCellStyle>

                    <SortedAscendingHeaderStyle BackColor="#6D95E1"></SortedAscendingHeaderStyle>

                    <SortedDescendingCellStyle BackColor="#E9EBEF"></SortedDescendingCellStyle>

                    <SortedDescendingHeaderStyle BackColor="#4870BE"></SortedDescendingHeaderStyle>
                </asp:GridView>
            </div>
            <br />
            <pagination:Page ID="paging" runat="server" />
        </asp:Panel>
    </cc1:UmbracoPanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        <% foreach (var properties in this.listReportProperties)
           { %>
            <% if (properties.DataType == ReportPropertyDataType.Date)
                { %>
                    jQuery(document).ready(function () {
                        jQuery('#<%= properties.Alias %>').umbDateTimePicker(false, 'Choose Date', 'No date chosen', 'Clear Date'); jQuery('#<%= properties.Alias %>').mask('9999-99-99');
                    });
            <% } %>
        <% } %>
        //]]>
    </script>
</asp:Content>
