<%@ Page Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="ReportQueryAdd.aspx.cs" Inherits="OgilvyOne.Backend.Reports.ReportQueryAdd" %>

<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <cc1:TabView ID="TabViewInfo" runat="server" Width="552px" Height="692px" />
    <cc1:Pane ID="pane1" runat="server">
        <asp:ValidationSummary runat="server" ID="ValidationSummaryDetails" ValidationGroup="query" DisplayMode="BulletList" CssClass="error" HeaderText="There were errors:" />

        <div class="propertyItem">
            <div class="propertyItemheader">Name</div>
            <div class="propertyItemContent">
                <asp:TextBox ID="txb_Name" CssClass="propertyFormInput" runat="server" MaxLength="200" Width="440"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_Name" ValidationGroup="query" ControlToValidate="txb_Name" ErrorMessage="Name" Text="Name is required" runat="server" SetFocusOnError="true" />
            </div>
        </div>
        <div class="propertyItem">
            <div class="propertyItemheader">Query count</div>
            <div class="propertyItemContent">
                <asp:TextBox ID="txb_QueryCount" runat="server" TextMode="MultiLine" Width="700" Height="160"></asp:TextBox>
            </div>
        </div>
        <div class="propertyItem">
            <div class="propertyItemheader">Query data</div>
            <div class="propertyItemContent">
                <asp:TextBox ID="txb_QueryData" runat="server" TextMode="MultiLine" Width="700" Height="260"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="rfv_QueryData" ValidationGroup="query" ControlToValidate="txb_QueryData" ErrorMessage="Query data" Text="Query data is required" runat="server" SetFocusOnError="true" />
            </div>
        </div>
        <div class="propertyPaneFooter">-</div>
    </cc1:Pane>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
