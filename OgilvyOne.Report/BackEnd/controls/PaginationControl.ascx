<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaginationControl.ascx.cs" Inherits="OgilvyOne.Report.Backend.controls.PaginationControl" %>
<% if (this.CurrentPage != null) 
   { %>
    <div hg-pagination="true" style="display:block;">
        <div class="listpaging" style="display:block;float:left;margin-top:3px;">
            <asp:Repeater ID="rpPage" runat="server" OnItemDataBound="rpPage_ItemDataBound">
                <HeaderTemplate>
                    <asp:HyperLink runat="server" ID="pageLink"></asp:HyperLink>
                    ...
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink runat="server" ID="pageLink"></asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    ...
                <asp:HyperLink runat="server" ID="pageLink"></asp:HyperLink>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <% if (this.CurrentPage.TotalPage > 1)
           { %>
        <div style="float: right; margin-left: 6px;" hg-paging="true">
            <input type="text" name="page" style="width: 30px;" hg-data="page" hg-pagemax="<%= this.CurrentPage.TotalPage %>" placeholder="<%= this.CurrentPage.CurrentPage %>" />
            /<%= this.CurrentPage.TotalPage %>
            <input type="button" value="Go" href="<%=this.GetUrlNotPage()%>" hg-data="button" />
        </div>
        <% } %>
    </div>
    <script type="text/javascript">
        $(function () {
            var paging = $('[hg-paging="true"]');
            paging.find('[hg-data="button"]').click(function () {
                var href = $(this).attr('href');
                var inputpage = paging.find('[hg-data="page"]');
                var pagemax = inputpage.attr('hg-pagemax');
                var page = parseInt(inputpage.val());

                if (page < 1) {
                    page = 1;
                }
                if (page > pagemax) {
                    page = pagemax;
                }
                window.location = href + page;
            });
        });
    </script>
<% } %>