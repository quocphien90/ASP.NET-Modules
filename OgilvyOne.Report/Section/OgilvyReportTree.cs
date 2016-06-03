using OgilvyOne.Report.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.businesslogic;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;

namespace OgilvyOne.Report.Section
{
    [Tree("ogilvyReportsApp", "OgilvyReportTree", "Report", sortOrder: 0)]
    public class OgilvyReportTree : BaseTree, ITree
    {
        public OgilvyReportTree(string application) : base(application) { }
        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.Icon = FolderIcon;
            rootNode.OpenIcon = FolderIconOpen;
            rootNode.NodeType = "init" + TreeAlias;
            rootNode.NodeID = "init";
        }

        protected override void CreateRootNodeActions(ref List<IAction> actions)
        {
            actions.Clear();
            actions.Add(umbraco.BusinessLogic.Actions.ActionRefresh.Instance);
        }
        protected override void CreateAllowedActions(ref List<IAction> actions)
        {
            actions.Clear();
            actions.Add(umbraco.BusinessLogic.Actions.ActionRefresh.Instance);
        }

        public override void Render(ref XmlTree tree)
        {
            // Create tree node to allow sending a newsletter
            tree.Clear();
            var node = XmlTreeNode.Create(this);

            //node.Text = "Active Member Scheme";

            //node.Icon = "settingDataType.gif";
            //node.OpenIcon = "settingDataType.gif";

            //node.Action = "javascript:openOgilvyAciteMemberScheme()";

            //tree.Add(node);

            //----------------------------------
            ReportProvider db = new ReportProvider();
            int total = 0;

            foreach (var query in db.SelectReportQuery(new ReportQueryCondition() { Status = ReportQueryStatus.Approve }, 0, int.MaxValue, out total))
            {
                node = XmlTreeNode.Create(this);

                node.Text = string.Format("{0}", query.Name);

                node.Icon = "settingDataType.gif";
                node.OpenIcon = "settingDataType.gif";

                node.Action = string.Format("javascript:openOgilvyReport({0})", query.ID);

                tree.Add(node);
            }
        }
        public override void RenderJS(ref System.Text.StringBuilder Javascript)
        {
//            Javascript.Append(@"function openOgilvyAciteMemberScheme() {
//				parent.right.document.location.href = 'Reports/ReportActiveMemberScheme.aspx';
//			}");

            //--------------------------------------
            Javascript.Append(@"function openOgilvyReport(id) {
				parent.right.document.location.href = 'Reports/ReportFromQuery.aspx?queryreportid=' + id;
			}");
        }
    }
    [Tree("ogilvyReportsApp", "OgilvyReportConfigTree", "Config", sortOrder: 1)]
    public class OgilvyReportConfigTree : BaseTree, ITree
    {
        public OgilvyReportConfigTree(string application) : base(application) { }
        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.Icon = FolderIcon;
            rootNode.OpenIcon = FolderIconOpen;
            rootNode.NodeType = "init" + TreeAlias;
            rootNode.NodeID = "init";
        }

        protected override void CreateRootNodeActions(ref List<IAction> actions)
        {
            actions.Clear();
            actions.Add(umbraco.BusinessLogic.Actions.ActionRefresh.Instance);
        }
        protected override void CreateAllowedActions(ref List<IAction> actions)
        {
            actions.Clear();
            actions.Add(umbraco.BusinessLogic.Actions.ActionRefresh.Instance);
        }

        public override void Render(ref XmlTree tree)
        {
            // Create tree node to allow sending a newsletter
            tree.Clear();
            var node = XmlTreeNode.Create(this);

            node.Text = "Queries";

            node.Icon = "settingDataType.gif";
            node.OpenIcon = "settingDataType.gif";

            node.Action = "javascript:openOgilvyReportQueryList()";

            tree.Add(node);
        }
        public override void RenderJS(ref System.Text.StringBuilder Javascript)
        {
            Javascript.Append(@"function openOgilvyReportQueryList() {
				parent.right.document.location.href = 'Reports/ReportQueriesList.aspx';
			}");
        }
    }

	
}