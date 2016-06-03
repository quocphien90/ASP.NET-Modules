<%@ Page Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="ReportQueriesList.aspx.cs" Inherits="OgilvyOne.Backend.Reports.ReportQueriesList" %>

<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <cc1:UmbracoPanel ID="UmbracoPanel1" runat="server" Text="Report list queries" hasMenu="true">
        <cc1:Pane ID="paneView" runat="server">
            <table rules="rows" border="0" class="members_table" style="table-layout: fixed">
                <colgroup>
                    <col width="20%" />
                    <col width="42" />
                    <col width="70" />
                    <col width="60" />
                    <col width="70" />
                    <col width="60" />
                    <col width="50" />
                </colgroup>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Status</th>
                        <th>Created date</th>
                        <th>Created by</th>
                        <th>Modified date</th>
                        <th>modified by</th>
                        <th></th>
                    </tr>
                </thead>
                <asp:Repeater runat="server" ID="rp_list" OnItemDataBound="rp_list_ItemDataBound" OnItemCommand="rp_list_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td><a href="ReportQueryEdit.aspx?id=<%# Eval("ID") %>">Edit</a> - <%# Eval("Name") %></td>
                            <td><%# Eval("Status") %></td>
                            <td><%# Eval("CreatedDate") %></td>
                            <td><%# Eval("CreatedBy") %></td>
                            <td><%# Eval("ModifiedDate") %></td>
                            <td><%# Eval("ModifiedBy") %></td>
                            <td align="right"><asp:Button ID="btn_changestatus" runat="server" CommandName="ChangeStatus" CommandArgument='<%# Eval("ID") %>'></asp:Button></td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alt">
                            <td><a href="ReportQueryEdit.aspx?id=<%# Eval("ID") %>">Edit</a> - <%# Eval("Name") %></td>
                            <td><%# Eval("Status") %></td>
                            <td><%# Eval("CreatedDate") %></td>
                            <td><%# Eval("CreatedBy") %></td>
                            <td><%# Eval("ModifiedDate") %></td>
                            <td><%# Eval("ModifiedBy") %></td>
                            <td align="right"><asp:Button ID="btn_changestatus" runat="server" CommandName="ChangeStatus" CommandArgument='<%# Eval("ID") %>'></asp:Button></td>
                        </tr>
                    </AlternatingItemTemplate>
                </asp:Repeater>
            </table>
        </cc1:Pane>
    </cc1:UmbracoPanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
