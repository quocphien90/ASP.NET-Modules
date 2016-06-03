<%@ Page Title="" Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Ogilvy.Backend.FileManager.Default" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link href="/css/jquery-ui.css" type="text/css" rel="stylesheet" />
	<link href="/scripts/plupload/jquery.ui.plupload/css/jquery.ui.plupload.css" type="text/css" rel="stylesheet" />
	<style type="text/css">
		td.file-item {
		}
		td.file-item-select,
		td:hover.file-item {
			background-color:#AAA;
		}
		.file-name {
			max-width:130px;
			word-break:break-all;
		}		
		.page_number {
			padding-left:5px;
			padding-right:5px;
		}
	</style>
	<script type="text/javascript" src="/scripts/plupload/plupload.full.min.js"></script>
	<script type="text/javascript" src="/scripts/plupload/i18n/vi.js"></script>
	<script type="text/javascript" src="/scripts/plupload/jquery.ui.plupload/jquery.ui.plupload.js"></script>
	<script type="text/javascript" src="/scripts/hg.backend.core.js" ></script>
	<script type="text/javascript">
		var cmdAddId = '<%=cmdAdd.ClientID%>';
	    var cmdAddFolderId = '<%=cmdAddFolder.ClientID%>';
	    var cmdDownload = '<%=cmdDownload.ClientID%>';
		var query_Id = '<%=Request.QueryString["ID"] %>';		
		var pageUrl = '<%="Default.aspx?ID=" + Request.QueryString["ID"] %>';		
	</script>
	<script type="text/javascript">
		$(function () {
			$('[hg-item]').click(function () {
				$('[hg-item]').each(function () {
					$(this).removeClass('file-item-select')
				});
				$(this).addClass('file-item-select')
				$('#divInfo').html($(this).find('[hg-item-info]').html());
			});
			$('[hg-item]').each(function () {
				if ($(this).find('[name="FolderKey"]').length > 0) {
					$(this).dblclick(function () {
						window.location.href = pageUrl + "&path=" + encodeURI($(this).find('[name="FolderKey"]').val());
					});
				}
			});
			$('#' + cmdAddId).click(function(){
				$('#divUploader').dialog({
					title : "Upload file",
					width: 550,
					modal: true,
					close: function () {
						window.location.href = window.location.href;
					}
				});
			});
			$('#' + cmdAddFolderId).click(function () {
				$('#divNewFolder').dialog({
					title: "New Folder in " + ($('#divNewFolder').attr('popup-title') || "Root"),
					width: 550,
					modal: true,
					buttons: [
						{
							text: "Create",
							click: function () {
								saveFolder(this);
							}
						}
					],
					close: function () {
						window.location.href = window.location.href;
					}
				});
			});
		
			function saveFolder(con) {
				con = $(con);
				$.post("/umbraco/surface/FileManager/NewFolder?ID=" + query_Id, { path: $('#folderPath').val(), name: $('#folderName').val() }, function (re) {
					if (re.HasError) {
						alert(re.Error);
						return;
					}
					//UmbClientMgr.mainTree().reloadActionNode(true, true);
					//jQuery(window.top).trigger("nodeRefresh", []);
					window.location.href = window.location.href;
				});
			}

			
		});

	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<cc1:UmbracoPanel ID="panel1" runat="server" Text="File manager " hasMenu="true"> 
	<cc1:MenuImageButton runat="server" ID="cmdDelete" ImageUrl="~/umbraco/images/editor/delete.png" ToolTip="Delete check items" AltText="Delete check items" OnClick="cmdDelete_Click" OnClientClick="return confirm('Bạn có chắc sẽ xóa tất cả file và folder (nếu có folder nó sẽ xóa cả folder con hoặc cháu của folder đó) đang chọn. Lưu ý việc xóa này sẽ không thể phục hồi. Cẩn thận trước khi xóa');"/>		
	<cc1:MenuImageButton runat="server" ID="cmdAdd" ImageUrl="~/umbraco/images/editor/Add.png" ToolTip="Add" AltText="Add" OnClientClick="return false;" />	
	<cc1:MenuImageButton runat="server" ID="cmdAddFolder" ImageUrl="~/umbraco/images/editor/folder_new.png" ToolTip="New Folder" AltText="New Folder" OnClientClick="return false;" />	
    <cc1:MenuImageButton runat="server" ID="cmdDownload" ImageUrl="~/umbraco/images/editor/download.png" ToolTip="Download" AltText="Download" OnClick="cmdDownload_Click" OnClientClick="return confirm('Mỗi lần chỉ được chọn 1 file để tải về')" />	
	<asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Create" />
	<cc1:Pane runat="server" ID="pnlContent">
		<div id="divInfo" style="min-height:30px;"></div>
	</cc1:Pane>
	<cc1:Pane ID="pane1" runat="server">    		
	<asp:ListView runat="server" ID="rpData" DataSourceID="ObjectDataSource1" GroupItemCount="5">
		<EmptyDataTemplate>
			<table runat="server" style="" cellpadding="0" cellspacing="0">
				<tr>
					<td>No data was returned.</td>
				</tr>
			</table>
		</EmptyDataTemplate>
		<EmptyItemTemplate>
			<td runat="server" />
		</EmptyItemTemplate>
		<GroupTemplate>
			<tr id="itemPlaceholderContainer" runat="server">
				<td id="itemPlaceholder" runat="server"></td>
			</tr>
		</GroupTemplate>
		<ItemTemplate>
			<td runat="server" class="file-item"  hg-item='<%# Eval("FullUrl") %>'>				
				<div style="text-align:center">
					<asp:PlaceHolder runat="server" ID="PlaceHolder1" Visible='<%#Convert.ToBoolean(Eval("IsFolder")) == false %>'>
						<a href="<%# Eval("FullUrl") %>" target="_blank">
							<asp:Image ID="ThumbLabel" runat="server" Width='<%# Unit.Parse(Eval("Width", "{0}px")) %>' Height='<%# Unit.Parse(Eval("Height", "{0}px")) %>' ImageUrl='<%# Eval("Thumb") %>' AlternateText='<%# Eval("Thumb") %>' />
						</a>
					</asp:PlaceHolder>
					<asp:PlaceHolder runat="server" ID="PlaceHolder2" Visible='<%#Convert.ToBoolean(Eval("IsFolder")) == true %>'>
						<input type="hidden" name="FolderKey" value="<%#Eval("Key") %>" />
						<img src="/umbraco/Images/FileManager/folder.png" alt="folder" width="125px" />
					</asp:PlaceHolder>
				</div>
				<div class="file-name">
					<asp:Label ID="NameLabel" runat="server" Text='<%# Eval("Name") %>'   />
				</div>
				<div style="display: none" hg-item-info="true">
					<asp:Literal ID="FullUrlLabel" runat="server" Text='<%# Eval("FullUrl") %>' Mode="Encode" />
				</div>
				<div>
					<asp:HiddenField runat="server" ID="tbxKey" Value='<%#Eval("Key") %>' />
					<asp:CheckBox runat="server" ID="chkCheck"  />
					<asp:PlaceHolder runat="server" ID="plhSize" Visible='<%#Convert.ToBoolean(Eval("IsFolder")) == false %>'>
						Size :
						<asp:Label ID="SizeLabel" runat="server" Text='<%# Eval("Size","{0:0,##0}") %>' />
					</asp:PlaceHolder>
				</div>
			</td>
		</ItemTemplate>
		<LayoutTemplate>
			<table id="groupPlaceholderContainer" runat="server" border="0" style="" cellpadding="0" cellspacing="5">
				<tr id="groupPlaceholder" runat="server">
				</tr>
			</table>
		</LayoutTemplate>
	</asp:ListView>
	<asp:DataPager ID="DataPager1" runat="server" PagedControlID="rpData">
		<Fields>
			<asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
			<asp:NumericPagerField ButtonCount="15" NumericButtonCssClass="page_number" CurrentPageLabelCssClass="page_number"   />
			<asp:NextPreviousPagerField ButtonType="Button" ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
		</Fields>
	</asp:DataPager>
    	<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetListItem" TypeName="Ogilvy.Backend.FileManager.Default+FileManagerDataObject" OnSelected="ObjectDataSource1_Selected" OnObjectCreating="ObjectDataSource1_ObjectCreating">
			<SelectParameters>
				<asp:QueryStringParameter Name="path" QueryStringField="path" Type="String" />
			</SelectParameters>
		</asp:ObjectDataSource>
<%--		<%=(new System.Web.Mvc.UrlHelper(Request.RequestContext)).Action("Upload","FileManager") %>--%>
    </cc1:Pane>
	<div>
		<div></div>
		<div></div>
	</div>
  </cc1:UmbracoPanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
	<script language="javascript" type="text/javascript">
		// Convert divs to queue widgets when the DOM is ready
		$(function () {
			$("#uploader").plupload({
				// General settings
				runtimes: 'html5,flash,silverlight',
				max_file_size: '4mb',
				url: '/umbraco/surface/FileManager/Upload?ID=' + query_Id,
				unique_names: true,
				// Specify what files to browse for
				//			filters: [{ title: "Image files", extensions: "jpg,gif,png,xls,xlsx" },
				//					{ title: "Zip files", extensions: "zip" }],
				multipart_params: { Path: '<%=Request["path"] %>', checksum: 'Checksum' },
				// Flash settings
				flash_swf_url: '/scripts/plupload/plupload.flash.swf',
				// Silverlight settings
				silverlight_xap_url: '/scripts/plupload/plupload.silverlight.xap'
			});

			var uploader = $('#uploader').plupload('getUploader');
			uploader.bind('StateChanged', function (up) {
				/*if (uploader.files.length === (uploader.total.uploaded + uploader.total.failed)) {
					
				}
				if(up.state === plupload.UPLOADING){
					$('a[id=uploader_browse]')
						.css("opacity", "0.5")
						.css("-moz-opacity", "0.5")
						.css("filter", "alpha(opacity=50)");
					$('input[type=file]').attr("disabled", true); 
					if($("div[id$='_flash_container']").length > 0){
						$("div[id$='_flash_container']").css("z-index", "-1");
					}
				}
				else if(up.state === plupload.STOPPED){
					$('a[id=uploader_browse]')
						.css("opacity", "1.0")
						.css("-moz-opacity", "1.0")
						.css("filter", "alpha(opacity=100)");
					$('input[type=file]').removeAttr("disabled"); 
					if($("div[id$='_flash_container']").length > 0){
						$("div[id$='_flash_container']").css("z-index", "99999");
					}
				}*/
			});
			if($('#uploader > div.plupload').not('.flash').length > 0)
			{
				$('#uploader > div.plupload').not('.flash').css('z-index','1050');
				$('#uploader_browse').css({"z-index" : "1051"});
			}

		});
	</script>
	<div style="display:none;" id="divUploader">
		<div id="uploader" style="min-width:500px;">
			<p>You browser doesn't have Flash, Silverlight, Gears, BrowserPlus or HTML5 support.</p>
		</div>
	</div>
	<div style="display:none;" id="divNewFolder" popup-title="<%=Request.QueryString["path"] %>">
		<div>Tên Folder</div>
		<input type="hidden" id="folderPath" name="folderPath" value="<%=Request.QueryString["path"] %>"/>
		<input type="text" id="folderName" name="folderName" style="width:90%" />
	</div>
</asp:Content>
