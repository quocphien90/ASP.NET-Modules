using System;
using System.Collections.Generic;
using System.Text;

namespace OgilvyOne.MSSQL
{
	/// <summary>
	/// Bảng lỗi
	/// </summary>
	public class SqlErrorCode
	{
		/// <summary>
		/// Dữ liệu bị trùng lập.
		/// </summary>
		public const int Duplicate = 2627;
		/// <summary>
		/// Dữ liệu vượt quá quy định cho phép.
		/// </summary>
		public const int Overflow = 8152;
		/// <summary>
		/// Kiểu dữ liệu không đúng định dang.
		/// </summary>
		public const int Type = 245;
		/// <summary>
		/// Dữ liệu là DBNull.value.
		/// </summary>
		public const int DBNull = 515;
		/// <summary>
		/// Dữ liệu là null.
		/// </summary>
		public const int Null = 8178;
		/// <summary>
		/// Khi xóa dữ liệu có quan hệ No Action.
		/// </summary>
		public const int Relate = 547;
	}
}
