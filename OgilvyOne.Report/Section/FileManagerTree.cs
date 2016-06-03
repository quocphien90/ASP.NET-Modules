using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco;
using umbraco.businesslogic;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic;
using OgilvyOne.Backend.FileManager;

namespace OgilvyOne.FileManager.Sections
{
    [Tree("ogilvyConfigureApp", "FileManagerTree", "File Manager", sortOrder: 1)]
	public class FileManagerTree : BaseTree, ITree
	{
		//ActionCreateFolder _ActionCreateFolder;
		public FileManagerTree(string application) : base(application) {  }

		protected override void CreateRootNode(ref XmlTreeNode rootNode)
		{
			rootNode.Icon = FolderIcon;
			rootNode.OpenIcon = FolderIconOpen;
			rootNode.Action = "javascript:openOgilvyFileManagerConfig();"; 
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
			//actions.Add(umbraco.BusinessLogic.Actions.ActionNew.Instance);
			//actions.Add(umbraco.BusinessLogic.Actions.ActionNewFolder.Instance);
			//actions.Add(_ActionCreateFolder);
			actions.Add(umbraco.BusinessLogic.Actions.ActionRefresh.Instance);
		}

		public override void Render(ref XmlTree tree)
		{
			var list = FileManagerHelper.GetListFileManagerConfig(FileManagerConfigStatus.Active);
			foreach (var item in list)
			{
				XmlTreeNode xNode = XmlTreeNode.Create(this);
				xNode.Text = item.Name;
				xNode.Icon = FolderIcon;
				xNode.OpenIcon = FolderIconOpen;
				xNode.NodeType = this.TreeAlias;
				xNode.Source = this.GetTreeServiceUrl(item.ID.ToString());
				xNode.HasChildren = false;
				xNode.Action = string.Format("javascript:openOgilvyFileManager('{0}');", item.ID);
				AddNode(tree, xNode);
			}
		}

		private void AddNode(XmlTree tree,XmlTreeNode xNode)
		{
			OnBeforeNodeRender(ref tree, ref xNode, EventArgs.Empty);
			if (xNode != null)
			{
				tree.Add(xNode);
				OnAfterNodeRender(ref tree, ref xNode, EventArgs.Empty);
			}
		}
		
		public override void RenderJS(ref System.Text.StringBuilder Javascript)
		{
			Javascript.Append(@"function openOgilvyFileManager(Id) {
				parent.right.document.location.href = 'FileManager/Default.aspx?ID=' + Id;
			}");
			Javascript.Append(@"function openOgilvyFileManagerConfig() {
				parent.right.document.location.href = 'FileManager/ConfigPage.aspx';
			}");
		}
		/*
		public class ActionCreateFolder : IAction
		{

			public string Alias
			{
				get { return "CreateFolder"; }
			}

			public bool CanBePermissionAssigned
			{
				get { return false; }
			}

			public string Icon
			{
				get { return ".sprCreateFolder"; }
			}

			public string JsFunctionName
			{
				get { return "showNewFolderInFileManager()"; }
			}

			public string JsSource
			{
				get { return "/scripts/Ogilvy.backend.fileManager.js"; }
			}

			public char Letter
			{
				get { return 'F'; }
			}

			public bool ShowInNotifier
			{
				get { return false; }
			}
		}
		 * */
	}

}