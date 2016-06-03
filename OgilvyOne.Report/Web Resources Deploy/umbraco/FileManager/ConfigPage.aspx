<%@ Page Title="" Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="ConfigPage.aspx.cs" Inherits="Ogilvy.Backend.FileManager.ConfigPage" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.panel_table {
			padding:4px;
		}
		.table {
			width:100%;
		}
		.table th {
			background-color:#0ff;

		}
		.table td, .table th {
			padding:4px;
		}
	</style>
	<script type="text/javascript" src="../js/hg.backend.core.js" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
<cc1:UmbracoPanel ID="umbracoPanel" runat="server" Text="File manager config" hasMenu="true">
	<asp:ValidationSummary runat="server" ID="valSummary" />
	<asp:Panel runat="server" ID="pnlList" CssClass="panel_table">
		<asp:GridView runat="server" ID="grdData" AutoGenerateColumns="False" CssClass="table" OnRowCommand="grdData_RowCommand">
			<Columns>
				<asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
				<asp:BoundField DataField="Path" HeaderText="Path" SortExpression="Path" />
				<asp:BoundField DataField="Maxrecord" HeaderText="Maxrecord" SortExpression="Maxrecord" />
				<asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" InsertVisible="False" ReadOnly="True" />
				<asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" DataFormatString="{0:dd/MM/yyyy hh:mm}" InsertVisible="False" ReadOnly="True" />
				<asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
				<asp:TemplateField>
					<ItemTemplate>
						<asp:LinkButton runat="server" Text="Active" ID="cmdActive" CommandName="Active" CommandArgument='<%#Eval("ID") %>' Visible='<%# ((OgilvyOne.Backend.FileManager.FileManagerConfigStatus)Eval("Status")) == OgilvyOne.Backend.FileManager.FileManagerConfigStatus.UnActive %>'/>
						<asp:LinkButton runat="server" Text="UnActive" ID="cmdUnActive" CommandName="UnActive" CommandArgument='<%#Eval("ID") %>' Visible='<%# ((OgilvyOne.Backend.FileManager.FileManagerConfigStatus)Eval("Status")) == OgilvyOne.Backend.FileManager.FileManagerConfigStatus.Active %>'/>
						<asp:LinkButton runat="server" Text="Edit" ID="cmdEdit" CommandName="EditItem" CommandArgument='<%#Eval("ID") %>' />
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
	</asp:Panel>
	<cc1:Pane runat="server" ID="pnlForm" Visible="false">
        <div class="propertyItem">
            <div class="propertyItemheader">Name</div>
            <div class="propertyItemContent">
                <asp:HiddenField ID="txbID" runat="server" Value='<%#Model.ID %>'></asp:HiddenField>
                <asp:TextBox ID="txbName" runat="server" Width="400px" Text='<%#Model.Name %>'></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator1"  runat="server" ControlToValidate="txbName" ErrorMessage="Name bắt buộc" Text="*"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="propertyItem">
            <div class="propertyItemheader">Path (có dấu ~)</div>
            <div class="propertyItemContent">
                <asp:TextBox ID="txbPath" runat="server" Width="400px" Text='<%#Model.Path %>'></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator2"  runat="server" ControlToValidate="txbPath" ErrorMessage="Path bắt buộc" Text="*"></asp:RequiredFieldValidator>
 				<asp:CustomValidator ID="CustomValidator2"  runat="server" ControlToValidate="txbMaxrecord" ErrorMessage="Path hợp lệ: phải có dấu ~ phía trước" Text="*" ></asp:CustomValidator>
           </div>
        </div>
        <div class="propertyItem">
            <div class="propertyItemheader">Số item</div>
            <div class="propertyItemContent">
                <asp:TextBox ID="txbMaxrecord" runat="server" Width="400px" Text='<%#Model.Maxrecord.ToString() %>'></asp:TextBox>				
				<asp:CustomValidator ID="CustomValidatorMaxrecord"  runat="server" ControlToValidate="txbMaxrecord" ErrorMessage="Số item sai" Text="*" ></asp:CustomValidator>
            </div>
        </div>		
        <div class="propertyItem">
            <div class="propertyItemheader">Status</div>
            <div class="propertyItemContent">
                <asp:DropDownList ID="cboStatus" runat="server" Width="400px">					
                </asp:DropDownList>
            </div>
        </div>
	</cc1:Pane>

</cc1:UmbracoPanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
