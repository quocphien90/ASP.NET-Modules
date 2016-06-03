using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.businesslogic;
using umbraco.interfaces;

namespace OgilvyOne.Report.Section
{
   [Application("ogilvyReportsApp", "Ogilvy Reports", "icon-reports.jpg", sortOrder: 16)]
	public class OgilvyReportSection : IApplication
	{
	}
   [Application("ogilvyConfigureApp", "Ogilvy Configure", "icon-config.jpg", sortOrder: 14)]
   public class OgilvyConfigureSection : IApplication
   {
   }

}