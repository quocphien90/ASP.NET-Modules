<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ElmahDetail.aspx.cs" Inherits="OOUmbracoElmah.ElmahDetail" MasterPageFile="~/umbraco/masterpages/umbracopage.master"%>

<%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
<umb:UmbracoPanel runat="server" hasMenu="false" Text="Exception Detail">

   <asp:Literal runat="server" ID="Output" />

</umb:UmbracoPanel>
</asp:Content>
