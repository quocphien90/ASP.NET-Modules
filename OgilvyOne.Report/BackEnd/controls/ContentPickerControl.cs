using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco;

namespace OgilvyOne.Report.Backend.controls
{
	/*
	 * demo
	 * <%@ Register TagPrefix="hg" Namespace="Huggies.CoreUI.Backend.controls" Assembly="Huggies.CoreUI" %>
	*/
	public class ContentPickerControl : umbraco.uicontrols.TreePicker.BaseTreePicker
	{
		public string WindowTitle { get; set; }
		public override string ModalWindowTitle
		{
			get
			{
				if (string.IsNullOrWhiteSpace(WindowTitle))
					return "Choose Content";
				else
					return WindowTitle;
			}
		}

		public override string TreePickerUrl
		{
			get
			{
				//umbraco.cms.presentation.Trees.TreeService treeSvc = new umbraco.cms.presentation.Trees.TreeService();
				//treeSvc.App = "content";
				//treeSvc.TreeType = "content";
				//treeSvc.StartNodeID = 1140;
				//return treeSvc.GetPickerUrl();
				return umbraco.cms.presentation.Trees.TreeService.GetPickerUrl("content", "content");
			}
		}
	}

}