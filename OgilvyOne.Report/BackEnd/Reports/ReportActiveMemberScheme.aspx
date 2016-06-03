<%@ Page Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="ReportActiveMemberScheme.aspx.cs" Inherits="Huggies.CoreUI.Backend.Reports.ReportActiveMemberScheme" %>

<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols.DatePicker" Assembly="controls" %>
<%@ Register TagPrefix="pagination" TagName="Page" Src="../controls/PaginationControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="/umbraco/js/hg.backend.core.js"></script>
    <link href="/content/themes/default/css/backend.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <cc1:UmbracoPanel ID="UmbracoPanel1" runat="server" Text="Report data" hasMenu="true">
        <cc1:MenuImageButton runat="server" ID="cmdExport" ImageUrl="~/umbraco/images/exportDocumenttype.png" ToolTip="Export" AltText="Export" OnClick="cmdExport_Click" />
        <cc1:Pane ID="pane1" runat="server">
            <div class="search" hg-find="true">
                <div class="date" date-name="f_fromdate">
                    From date<br />
                    <cc2:DateTimePicker ID="f_fromdate" runat="server" ShowTime="false"></cc2:DateTimePicker>
                </div>
                <div class="date" date-name="f_todate">
                    To date<br />
                    <cc2:DateTimePicker ID="f_todate" runat="server" ShowTime="false"></cc2:DateTimePicker>
                </div>
                <div class="select">
                    Member type<br />
                    <select id="f_membertype" name="f_membertype">
                        <option value="">All</option>
                        <option value="Expert" <%= this.selectValue("Expert", "f_membertype") %>>Expert</option>
                        <option value="Moderator" <%= this.selectValue("Moderator", "f_membertype") %>>Moderator</option>
                        <option value="Member" <%= this.selectValue("Member", "f_membertype") %>>Member</option>
                    </select>
                </div>
                <div class="button">
                    <input type="button" value="Search" hg-find-command="true" href="ReportActiveMemberScheme.aspx" />
                </div>
            </div>
        </cc1:Pane>
        <cc1:Pane ID="pane2" runat="server" CssClass="mainScroll">
            <div style="width: 100%;">
                <div style="width: auto; overflow: scroll;">
                    <asp:GridView ID="gv_Data" runat="server" AutoGenerateColumns="false" CellPadding="4" PagerSettings-PageButtonCount="20" ForeColor="#333333" GridLines="None" OnDataBound="gv_Data_DataBound">
                        <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>

                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="Email" DataNavigateUrlFormatString="../huggiesMembers/MemberActionLogPage.aspx?f_Email={0}" DataTextField="ID" HeaderText="ID"></asp:HyperLinkField>
                            <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="../huggiesMembers/InfoMember.aspx?id={0}" DataTextField="Email" HeaderText="Email"></asp:HyperLinkField>
                            <asp:BoundField DataField="Status" HeaderText="Status"></asp:BoundField>
                            <asp:BoundField DataField="MemberType" HeaderText="MemberType"></asp:BoundField>
                            <asp:TemplateField ConvertEmptyStringToNull="False">
                                <HeaderTemplate>Login</HeaderTemplate>
                                <ItemTemplate>
                                    <a href="../huggiesMembers/MemberActionLogPage.aspx?f_Email=<%# Eval("Email") %>&f_Action=2&f_Fromdate=<%= string.IsNullOrWhiteSpace(Request.QueryString["f_fromdate"]) ? "" : string.Format("{0}+00:00",Request.QueryString["f_fromdate"]) %>&f_Todate=<%= string.IsNullOrWhiteSpace(Request.QueryString["f_todate"]) ? "" : string.Format("{0}+23:59",Request.QueryString["f_todate"]) %>"><%# Eval("Login") %></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ExpertPublic" HeaderText="ExpertPublic"></asp:BoundField>
                            <asp:BoundField DataField="ExpertUnPublic" HeaderText="ExpertUnPublic"></asp:BoundField>
                            <asp:BoundField DataField="ForumsPostPublic" HeaderText="ForumsPostPublic"></asp:BoundField>
                            <asp:BoundField DataField="ForumsPostUnPublic" HeaderText="ForumsPostUnPublic"></asp:BoundField>
                            <asp:BoundField DataField="ForumsCommentPublic" HeaderText="ForumsCommentPublic"></asp:BoundField>
                            <asp:BoundField DataField="ForumsCommentUnPublic" HeaderText="ForumsCommentUnPublic"></asp:BoundField>
                        </Columns>

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
            </div>
            <br />
            <pagination:Page ID="paging" runat="server" />
        </cc1:Pane>
    </cc1:UmbracoPanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
