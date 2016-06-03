using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Reflection;

namespace OgilvyOne.MSSQL
{
	/// <summary>
	/// Tương ứng với TABLEID trên sql.
	/// dùng để truyền mãng các ID lên sql
	/// </summary>
public class TableID  {

	public DataTable Data { get; private set; }
	/// <summary>
	/// Tạo một bảng ID rỗng với type ID là Guid
	/// </summary>
	public TableID() :this(typeof(Guid)) { }
	public TableID(Type type) {
		Data = new DataTable();
		Data.TableName = "TableID";
		Data.Columns.Add("ID", type);
	}

	public TableID(IEnumerable<Guid> ListID)
		: this(ListID, typeof(Guid)) {
		
	}
	public TableID(IEnumerable<long> ListID)
		: this(ListID, typeof(long))
	{

	}
	public TableID(IEnumerable ListID, Type type)
		: this(type) {
		if (ListID != null) {
			foreach (var id in ListID) {
				this.AddRow(id);
			}
		}
	}

	public void Load<T>(IEnumerable<T> List, string FieldID) {
		PropertyInfo pi = typeof(T).GetProperty(FieldID);
		if (pi.PropertyType.Equals(typeof(Guid))) {
			foreach (var item in List) {
				this.AddRow(pi.GetValue(item, null));
			}
		} else {
			throw new ArgumentException(string.Format("{0} not is Type", FieldID));
		}
	}

	public int Count {
		get {
			return Data.Rows.Count;
		}
	}
	public void AddRow(object ID) {
		Data.Rows.Add(ID);
	}
	public object this[int index] {
		get { return Data.Rows[index][0]; }
	}
}
}