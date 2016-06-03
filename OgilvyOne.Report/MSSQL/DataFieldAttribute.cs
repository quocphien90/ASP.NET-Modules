using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgilvyOne.MSSQL
{
	[AttributeUsageAttribute (AttributeTargets.Property)]
	public class DataFieldAttribute : Attribute
	{
		public DataFieldAttribute()
		{
			this.DataFieldName = null;
			CanCreateField =  CanEditField = true;
		}

		public DataFieldAttribute(string DataFieldName)
		{
			this.DataFieldName = DataFieldName;
			CanCreateField =  CanEditField = true;
		}

		public string DataFieldName { get; set; }

		/// <summary>
		/// Sẽ Add Parameter Create với dạng out put
		/// </summary>
		public bool CreateOutPut { get; set; }
		/// <summary>
		/// Sẽ Add Parameter Edit với dạng out put
		/// </summary>
		public bool EditOutPut { get; set; }
		/// <summary>
		/// Sẽ Add Parameter Create nếu là true, mặc định là true
		/// </summary>
		public bool CanCreateField { get; set; }
		/// <summary>
		/// Sẽ Add Parameter Edit nếu là true, mặc định là true
		/// </summary>
		public bool CanEditField { get; set; }
	}
}
