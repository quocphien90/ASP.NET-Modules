<%@ Page Language="C#" MasterPageFile="../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="ReportQueryEdit.aspx.cs" Inherits="OgilvyOne.Backend.Reports.ReportQueryEdit" %>

<%@ Import Namespace="OgilvyOne.Report.Model.Reports" %>

<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            //sort table
            $('table.members_table').find('tbody').sortable();
        });
        function dataOrder() {
            var hidden = $('#<%=hd_listID.ClientID%>');
            var listrow = $('table.members_table tbody tr');
            var listId = "";
            listrow.each(function () {
                var hdID = $(this).find('td:eq(0) input[type="hidden"]').val();
                if (listId.length == 0) {
                    listId = hdID;
                }
                else {
                    listId = listId + ',' + hdID;
                }
            });
            hidden.val(listId);
            return true;
        };
    </script>
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
        <cc1:PropertyPanel runat="server" Text="Status">
            <asp:DropDownList runat="server" ID="ddl_Status"></asp:DropDownList>
        </cc1:PropertyPanel>
        <div class="propertyPaneFooter">-</div>
    </cc1:Pane>

    <cc1:Pane runat="server" ID="panePropertiesCreateEdit" Visible="true">
        <asp:Button ID="btn_PropertiesAdd" runat="server" Text="Add properties" OnClick="btn_PropertiesAdd_Click" />
        <cc1:Pane runat="server" ID="panePropertiesInfo" Visible="false" Text="Add propertise">
            <asp:ValidationSummary runat="server" ID="ValidationSummaryProperties" ValidationGroup="properties" DisplayMode="BulletList" CssClass="error" HeaderText="There were errors:" />
            <asp:HiddenField runat="server" ID="hd_PropertiesID" />
            <cc1:PropertyPanel ID="PropertyPanel1" runat="server" Text="Name">
                <asp:TextBox ID="txb_PropertiesName" CssClass="propertyFormInput" runat="server" MaxLength="255" Width="440"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_PropertiesName" ValidationGroup="properties" ControlToValidate="txb_PropertiesName" ErrorMessage="Name" Text="Name is required" runat="server" SetFocusOnError="true" />
            </cc1:PropertyPanel>
            <cc1:PropertyPanel runat="server" Text="Alias">
                <asp:TextBox ID="txb_PropertiesAlias" CssClass="propertyFormInput" runat="server" MaxLength="255" Width="440"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_PropertiesAlias" ValidationGroup="properties" ControlToValidate="txb_PropertiesAlias" ErrorMessage="Alias" Text="Alias is required" runat="server" SetFocusOnError="true" />
            </cc1:PropertyPanel>
            <cc1:PropertyPanel runat="server" Text="Data type">
                <asp:DropDownList ID="ddl_PropertiesDataType" runat="server"></asp:DropDownList>
            </cc1:PropertyPanel>
            <cc1:PropertyPanel runat="server" Text="Mandatory">
                <asp:CheckBox ID="cb_PropertiesMandatory" runat="server" />
            </cc1:PropertyPanel>
            <cc1:PropertyPanel runat="server" Text="PreValue">
                <asp:TextBox ID="txb_PropertiesPreValue" CssClass="propertyFormInput" runat="server" MaxLength="500" Width="440"></asp:TextBox>
                <div style="padding-bottom: 10px;padding-top: 2px;">if Data type is Select, Example: Text:Value,Text:Value,Text:Value... Or Text,Text,Text...</div>
            </cc1:PropertyPanel>
            <cc1:PropertyPanel runat="server" Text="Description">
                <asp:TextBox ID="txb_PropertiesDescription" runat="server" TextMode="MultiLine" MaxLength="1000" Width="440" Height="120"></asp:TextBox>
            </cc1:PropertyPanel>
            <cc1:PropertyPanel runat="server" Text=" ">
                <asp:Button ID="btn_PropertiesSave" runat="server" Text="Save properties" OnClick="btn_PropertiesSave_Click" />
                <asp:Button ID="btn_PropertiesCancel" runat="server" Text="Cancel" OnClick="btn_PropertiesCancel_Click" />
            </cc1:PropertyPanel>
        </cc1:Pane>
    </cc1:Pane>

    <cc1:Pane runat="server" ID="panePropertiesView" Visible="true">
        <p> Drag and drop items to sort list</p>
        <asp:HiddenField ID="hd_listID" runat="server" />
        <table rules="rows" border="0" class="members_table" id="list_properties" style="table-layout: fixed; word-wrap: break-word;">
            <colgroup>
                <col width="20%" />
                <col width="16%" />
                <col width="8%" />
                <col width="10%" />
                <col width="20%" />
                <col width="12%"/>
                <col width="40px" />
            </colgroup>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Alias</th>
                    <th>Data type</th>
                    <th>Mandatory</th>
                    <th>PreValue</th>
                    <th>Description</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="rp_Propertise" OnItemCommand="rp_Propertise_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:LinkButton ID="lb_edit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton> - <%# Eval("Name") %>
                                <input type="hidden" name="hd_id" value="<%# Eval("ID") %>" />
                            </td>
                            <td><%# Eval("Alias") %></td>
                            <td><%# ((ReportPropertyDataType)Eval("DataType")).ToDisplay() %></td>
                            <td><%# Eval("Mandatory") %></td>
                            <td><%# Eval("DataPreValue") %></td>
                            <td><%# Eval("Description") %></td>
                            <td align="right"><asp:LinkButton ID="lb_remove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("ID") %>'>Remove</asp:LinkButton></td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alt">
                            <td>
                                <asp:LinkButton ID="lb_edit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton> - <%# Eval("Name") %>
                                <input type="hidden" name="hd_id" value="<%# Eval("ID") %>" />
                            </td>
                            <td><%# Eval("Alias") %></td>
                            <td><%# ((ReportPropertyDataType)Eval("DataType")).ToDisplay() %></td>
                            <td><%# Eval("Mandatory") %></td>
                            <td><%# Eval("DataPreValue") %></td>
                            <td><%# Eval("Description") %></td>
                            <td align="right"><asp:LinkButton ID="lb_remove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("ID") %>'>Remove</asp:LinkButton></td>
                        </tr>
                    </AlternatingItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </cc1:Pane>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript">
        $(function () {
            var proName = $("input[id$='txb_PropertiesName']");
            var proAlias = $("input[id$='txb_PropertiesAlias']");
            proName.on("keyup", function () {
                proAlias.val($(this).val().replace(/ /g, ''));
            });
            proAlias.on("keyup", function () {
                $(this).val($(this).val().replace(/ /g, ''));
            });
        });
    </script>
</asp:Content>
