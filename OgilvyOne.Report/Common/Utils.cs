using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web.Helpers;
using System.Security.Cryptography;

/// <summary>
/// Utils class.
/// </summary>
public static class Utils {

    public static umbraco.DataLayer.IParameter Parameter(this umbraco.DataLayer.ISqlHelper SqlHelper, string name, object value)
    {
        if (value == null)
        {
            return SqlHelper.CreateParameter(name, DBNull.Value);
        }
        else
        {
            return SqlHelper.CreateParameter(name, value);
        }
    }

	/// <summary>
	/// Writes a message to the log file.
	/// </summary>
	/// <param name="txt">Text to be logged</param>
	/// <returns>True on success or false on failure</returns>
	public static bool Log(string txt) {
		try {
			//Environment.CurrentDirectory
			umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Error, -1, txt);
		} catch { }
		return false;
	}
	public static bool Log(string name, Exception ex)
	{
		StringBuilder sb =new StringBuilder(name);
		sb.Append(" ");
		sb.Append(ex.ToString());
		return Log(sb.ToString());
	}
    /// <summary>
    /// convert to datetime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(string dateTime, string culture = "vi-VN" ){
        return Convert.ToDateTime(dateTime, System.Globalization.CultureInfo.GetCultureInfo(culture));
    }
	public static string ConvertDateTimeToString(DateTime? dateTime, string culture = "vi-VN")
	{
		if (dateTime.HasValue)
		{
			return Convert.ToString(dateTime, System.Globalization.CultureInfo.GetCultureInfo(culture));
		}
		return "";
	}

	/// <summary>
	/// Is the passed datetime a work day?
	/// </summary>
	/// <param name="dt">Input datetime</param>
	/// <returns>True if the passed datetime is Satuarday or Sunday otherwise return false</returns>
	public static bool IsWeekend(this DateTime dt) {
		return dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday;
	}

	/// <summary>
	/// Finds the first day of year of the specified day.
	/// </summary>
	/// <param name="year">Input year</param>
	/// <returns></returns>
	public static DateTime FirstDayOfYear(this int year) {
		return new DateTime(year, 1, 1);
	}

	/// <summary>
	/// Finds the first day of year of the specified day.
	/// </summary>
	/// <param name="dt">Input datetime</param>
	/// <returns></returns>
	public static DateTime FirstDayOfYear(this DateTime dt) {
		return dt.Year.FirstDayOfYear();
	}

	/// <summary>
	/// Finds the carry day of year of the specified day.
	/// </summary>
	/// <param name="year">Input year</param>
	/// <returns></returns>
	public static DateTime CarryDayOfYear(this int year) {
		return new DateTime(year, 3, 31, 23, 59, 0);
	}

	/// <summary>
	/// Finds the carry day of year of the specified day.
	/// </summary>
	/// <param name="dt">Input datetime</param>
	/// <returns></returns>
	public static DateTime CarryDayOfYear(this DateTime dt) {
		return dt.Year.CarryDayOfYear();
	}

	/// <summary>
	/// Converts all accent characters to ASCII characters.
	/// </summary>
	/// <param name="txt">Text that might have accent characters</param>
	/// <returns>Filtered text with replaced "nice" characters</returns>
	public static string NonAccent(this string txt) {
		if (!string.IsNullOrWhiteSpace(txt)) {
			string[] unicode = new string[15] {
"aAeEoOuUiIdDyY", "áàảãạăắằẳẵặâấầẩẫậ", "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ", "éèẻẽẹêếềểễệ", "ÉÈẺẼẸÊẾỀỂỄỆ",
"óòỏõọôốồổỗộơớờởỡợ", "ÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢ", "úùủũụưứừửữự", "ÚÙỦŨỤƯỨỪỬỮỰ", "íìỉĩị", "ÍÌỈĨỊ", "đ", "Đ", "ýỳỷỹỵ", "ÝỲỶỸỴ"
			};
			string[] utf8 = new string[15] {
"aAeEoOuUiIdDyY", "áàảãạăắằẳẵặâấầẩẫậ", "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ", "éèẻẽẹêếềểễệ", "ÉÈẺẼẸÊẾỀỂỄỆ",
"óòỏõọôốồổỗộơớờởỡợ", "ÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢ", "úùủũụưứừửữự", "ÚÙỦŨỤƯỨỪỬỮỰ", "íìỉĩị", "ÍÌỈĨỊ", "đ", "Đ", "ýỳỷỹỵ", "ÝỲỶỸỴ"
			};
			for (int i = 1; i < unicode.Length; i++) {
				for (int j = 0; j < unicode[i].Length; j++) {
					txt = txt.Replace(utf8[i][j], utf8[0][i - 1]).Replace(unicode[i][j], unicode[0][i - 1]);
				}
			}
			txt = Regex.Replace(Regex.Replace(txt, "[^a-zA-Z0-9_-]", "-", RegexOptions.Compiled), "-+", "-", RegexOptions.Compiled).Trim('-');
		} else {
			txt = string.Empty;
		}
		return txt;
	}
	public static string TrimVietNamMark(this string txt)
	{
		if (!string.IsNullOrWhiteSpace(txt))
		{
			string[] unicode = new string[15] {
"aAeEoOuUiIdDyY", "áàảãạăắằẳẵặâấầẩẫậ", "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ", "éèẻẽẹêếềểễệ", "ÉÈẺẼẸÊẾỀỂỄỆ",
"óòỏõọôốồổỗộơớờởỡợ", "ÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢ", "úùủũụưứừửữự", "ÚÙỦŨỤƯỨỪỬỮỰ", "íìỉĩị", "ÍÌỈĨỊ", "đ", "Đ", "ýỳỷỹỵ", "ÝỲỶỸỴ"
			};
			string[] utf8 = new string[15] {
"aAeEoOuUiIdDyY", "áàảãạăắằẳẵặâấầẩẫậ", "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ", "éèẻẽẹêếềểễệ", "ÉÈẺẼẸÊẾỀỂỄỆ",
"óòỏõọôốồổỗộơớờởỡợ", "ÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢ", "úùủũụưứừửữự", "ÚÙỦŨỤƯỨỪỬỮỰ", "íìỉĩị", "ÍÌỈĨỊ", "đ", "Đ", "ýỳỷỹỵ", "ÝỲỶỸỴ"
			};
			for (int i = 1; i < unicode.Length; i++)
			{
				for (int j = 0; j < unicode[i].Length; j++)
				{
					txt = txt.Replace(utf8[i][j], utf8[0][i - 1]).Replace(unicode[i][j], unicode[0][i - 1]);
				}
			}
			//txt = Regex.Replace(Regex.Replace(txt, "[^a-zA-Z0-9_-]", "-", RegexOptions.Compiled), "-+", "-", RegexOptions.Compiled).Trim('-');
		}
		else
		{
			txt = string.Empty;
		}
		return txt;
	}

	/// <summary>
	/// Strips HTML tags from provided text.
	/// </summary>
	/// <param name="txt">Input text</param>
	/// <returns>Stripped text</returns>
	public static string StripHtmlTags(this string txt) {
		if (txt == null) {
			return txt;
		}
		bool search = true;
		while (search) {
			int start = txt.IndexOf("<!--");
			int end = txt.IndexOf("-->");
			if (start != -1 && end > start) {
				txt = txt.Substring(0, start) + txt.Substring((end + 3), txt.Length - (end + 3));
			} else {
				search = false;
			}
		}
		return Regex.Replace(txt, "<[^>]+>", string.Empty, RegexOptions.Compiled);
	}

	/// <summary>
	/// Forces long text to wrap.
	/// </summary>
	/// <param name="txt">Input text</param>
	/// <param name="margin">Wrap margin</param>
	/// <returns>Wrapped text</returns>
	public static string Wrap(object txt, int margin) {
		List<string> lines = new List<string>();
		string s = Regex.Replace(StripHtmlTags(Convert.ToString(txt)), @"\s", " ", RegexOptions.Compiled).Trim();
		int start = 0, end;
		while ((end = start + margin) < s.Length) {
			while (s[end] != ' ' && end > start) {
				end -= 1;
			}
			if (end == start) {
				end = start + margin;
			}
			lines.Add(s.Substring(start, end - start));
			start = end + 1;
		}
		if (start < s.Length) {
			lines.Add(s.Substring(start));
		}
		return string.Join(" ", lines.ToArray());
	}


    //public static string RenderPartialView(this Controller controller, string viewName, object model)
    //{
    //    if (string.IsNullOrEmpty(viewName))
    //        viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
    //    controller.ViewData.Model = model;
    //    using (var sw = new StringWriter())
    //    {
    //        ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
    //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
    //        viewResult.View.Render(viewContext, sw);
    //        return sw.GetStringBuilder().ToString();
    //    }
    //}
	/// <summary>
	/// Grabs the Enum Display attribute.
	/// Usage: TestEnum.TestValue.ToDisplay()
	/// </summary>
	/// <param name="value">The given enumeration</param>
	/// <returns>The description attribute</returns>
	public static string ToDisplay(this Enum value)
	{
		if (value == null)
			return "";
		var field = value.GetType().GetField(value.ToString());
		if (field != null)
		{
			DisplayAttribute[] da = (DisplayAttribute[])(field.GetCustomAttributes(typeof(DisplayAttribute), false));
			return da.Length != 0 ? da[0].GetName() : value.ToString();
		}
		return string.Empty;
	}

	public static List<KeyValuePair<T, string>> EnumToList<T>(this Type value, T[] limit) {
		List<KeyValuePair<T, string>> kvp = new List<KeyValuePair<T, string>>();
		Array values = Enum.GetValues(value);
		foreach (var s in values) {
			if (Array.Exists(limit, k => k.Equals(Convert.ChangeType(s, typeof(T))))) {
				kvp.Add(new KeyValuePair<T, string>((T)Convert.ChangeType(s, typeof(T)), ((Enum)s).ToDisplay()));
			}
		}
		//kvp.Sort(delegate(KeyValuePair<T, string> x, KeyValuePair<T, string> y) { return x.Value.CompareTo(y.Value); });
		return kvp;
	}
	public static List<KeyValuePair<T, string>> EnumToList<T>(this Type value)
	{
		List<KeyValuePair<T, string>> kvp = new List<KeyValuePair<T, string>>();
		Array values = Enum.GetValues(value);
		foreach (var s in values)
		{
			kvp.Add(new KeyValuePair<T, string>((T)Convert.ChangeType(s, typeof(T)), ((Enum)s).ToDisplay()));
		}
		//kvp.Sort(delegate(KeyValuePair<T, string> x, KeyValuePair<T, string> y) { return x.Value.CompareTo(y.Value); });
		return kvp;
	}

	#region Hung

	/// <summary>
	/// Viết thường ký tự đầu tiên
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static string ToLowerFirstChar(this string text) {
		if (text.Length == 0) return "";
		string re = text[0].ToString().ToLower();
		re += text.Substring(1);
		return re;
	}

	public static string[] AnalyzeText(this string text) {
		List<string> kq = new List<string>();
		bool passSpace = false;
		string item = "";
		for (int i = 0; i < text.Length; i++) {
			if (text[i] == '"') {
				item = item.Trim();
				if (!string.IsNullOrEmpty(item)) {
					kq.Add(item.Trim());
					item = "";
				}
				passSpace = !passSpace;
				continue;
			} else if (text[i] == ' ') {
				if (passSpace) {
					item += " ";
					continue;
				} else {
					item = item.Trim();
					if (string.IsNullOrEmpty(item)) continue;
					kq.Add(item);
					item = "";
				}
			} else {
				item += text[i];
			}
		}
		item = item.Trim();
		if (!string.IsNullOrEmpty(item)) kq.Add(item);
		return kq.ToArray();
	}


	/// <summary>
	/// Analyse FullText Search 
	/// </summary>
	/// <param name="text">Text analyse</param>
	/// <returns></returns>
	public static string AnalyzeFullTextSearch(this string text) {
		string[] lst = AnalyzeText(text);
		string kq = "";
		bool notAddAND = false;
		for (int i = 0; i < lst.Length; i++) {
			string item = lst[i].Trim();
			if (item.Equals("or", StringComparison.OrdinalIgnoreCase)
				|| item.Equals("and", StringComparison.OrdinalIgnoreCase)) {
				kq += " " + item.ToUpper() + " ";
				notAddAND = true;
				continue;
			} else {
				if ((kq != "") && !notAddAND) {
					kq += " AND ";
				}
				notAddAND = false;
				kq += "\"" + item + "\"";
			}
		}
		return kq;
	}

	/// <summary>
	/// Generate Random String, with useSpecialChar = true
	/// </summary>
	/// <param name="length">Length of String Random, Condition Length greater than 0 And less than 100</param>
	/// <returns>Return String Random</returns>
	public static string GenerateRandomString(int length) {
		if (length > 100 || length <= 0) {
			length = 10;
		}
		Random r = new Random();
		string re = "";
		for (int i = 0; i < length; i++) {
			re += Convert.ToChar(r.Next(33, 122)).ToString(); // 33=! and 122=z
		}
		return re;
	}

	/// <summary>
	/// Generate Random String
	/// </summary>
	/// <param name="length">>Length of String Random, Condition Length greater than 0 And less than 100</param>
	/// <param name="useSpecialChar">true use char Special, false not use</param>
	/// <returns></returns>
	public static string GenerateRandomString(int length, bool useSpecialChar) {
		if (length > 100 || length <= 0) {
			length = 10;
		}
		if (useSpecialChar) {
			return GenerateRandomString(length);
		}
		Random r = new Random();
		string re = "";
		for (int i = 0; i < length; i++) {
			int randomNumber = r.Next(0, 61);
			if (randomNumber < 10) {
				re += Convert.ToChar(randomNumber + 48).ToString();
			} else if (randomNumber < 36) {
				//A = 65 ; Z = 90
				re += Convert.ToChar(randomNumber + 55).ToString();
			} else {
				//a = 97 ; z = 122
				re += Convert.ToChar(randomNumber + 61).ToString();
			}
		}
		return re;
	}

	/// <summary>
	/// Cut string by number char.
	/// </summary>
	/// <param name="content">String need cut.</param>
	/// <param name="numberChar">Number char need cut.</param>
	/// <returns>New string</returns>
	public static string CutStringByNum(this string content, int numberChar) {
		if (string.IsNullOrEmpty(content))
		{
			return content;
		}
		content = StripHtmlTags(content);
		int len = content.Length;
		if (String.IsNullOrEmpty(content) || len <= numberChar) return content.Trim();

		for (int i = 0; i < 10; i++) {
			numberChar = numberChar + 1;
			if ((numberChar >= content.Length)
				|| (numberChar < content.Length && content[numberChar] == ' ')) break;
		}
		content = content.Substring(0, numberChar);
		return content + (numberChar < len ? "..." : "");
	}

	public static string ConvertListToString<T>(this IEnumerable<T> list, string separate) {
		return ConvertListToString(list, separate, false);
	}


	/// <summary>
	/// Nối các phần tử trong mảng thành 1 chuổi
	/// </summary>
	/// <typeparam name="T">Type object (use tostring())</typeparam>
	/// <param name="list">Mảng object</param>
	/// <param name="separate">dấu phân cách giữa các phần tử</param>
	/// <returns></returns>
	public static string ConvertListToString<T>(this IEnumerable<T> list, string separate, bool useQuote) {
		StringBuilder result = new StringBuilder();
		foreach (T value in list) {
			if (result.Length != 0) {
				result.Append(separate);
			}
			if (useQuote) {
				result.Append("'" + value.ToString() + "'");
			} else {
				result.Append(value.ToString());
			}
		}
		return result.ToString();
	}

	public static List<T> ConvertStringToList<T>(this string stringInput, string separate) {
		List<T> result = new List<T>();
		string[] arr = stringInput.Split(new string[] { separate }, StringSplitOptions.RemoveEmptyEntries);
		for (var i = 0; i < arr.Length; i++) {
			result.Add((T)Convert.ChangeType(arr[i].Trim(), typeof(T)));
		}
		return result;
	}

	/// <summary>
	/// So sánh 2 chuổi và cho điểm. Điểm số càng lớn là càng trùng khớp.
	/// Thuật toán dùng matrix lượng giá. O(m*n) m, n chiều dài 2 chuổi
	/// </summary>
	/// <param name="s">Chuỗi thứ nhất</param>
	/// <param name="t">Chuỗi thứ hai</param>
	/// <param name="stringComparison">dạng so sách có quan tâm hoa thường</param>
	/// <returns>Số diểm trùng khớp của 2 chuổi</returns>
	public static int FuzzyMatch(string s, string t, StringComparison stringComparison) {
		int n = s.Length; //length of s
		int m = t.Length; //length of t
		int[,] d = new int[n, m]; // matrix
		int total = 0; // cost
		int equalsCost = 1;
		// Step 1
		//if (n == 0) return m;
		//if (m == 0) return n;
		int[] h = new int[n];
		int[] v = new int[m];
		// Step 2
		for (int i = 0; i < n; i++) h[i] = 0;
		for (int i = 0; i < m; i++) v[i] = 0;
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < m; j++) {
				if (t.Substring(j, 1).Equals(s.Substring(i, 1), stringComparison)) {
					if ((i == 0) || (j == 0)) {
						d[i, j] = equalsCost;
					} else {
						d[i, j] = equalsCost + d[i - 1, j - 1];
					}
				} else {
					d[i, j] = 0;
				}
				if ((h[i] < d[i, j]) && (v[j] < d[i, j])) {
					total += Math.Min(d[i, j] - h[i], d[i, j] - v[j]);
					h[i] = d[i, j];
					v[j] = d[i, j];
				}
			}
		}
		// Step 7
		return total;
	}

	/// <summary>
	/// Encode HTML.
	/// Chuyển đổi các ký tự xuống hàng thành &lt;br /&gt;
	/// </summary>
	/// <param name="plainText"></param>
	/// <returns></returns>
	public static string Br2nl(this string plainText) {
		plainText = HttpUtility.HtmlEncode(plainText);
		return plainText.Replace(Environment.NewLine, "<br/>");
	}

	/// <summary>
	/// Decode HTML.
	/// Chuyển đổi &lt;br /&gt; thành ký tự xuống hàng
	/// </summary>
	/// <param name="richText"></param>
	/// <returns></returns>
	public static string Nl2br(this string richText) {
		richText = HttpUtility.HtmlDecode(richText);
		return richText.Replace("<br/>", Environment.NewLine);
	}

	/// <summary>
	/// Đảo ngược một chuỗi
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	public static string Inverse(this string target) {
		char[] kq = new char[target.Length];
		int index = 0;
		for (int i = target.Length - 1; i >= 0; i--) {
			kq[index] = target[i];
			index++;
		}
		return new string(kq);
	}

    //public static PageInfo CalcBeginEnd(this Controller controller, int page) {
    //    int limit = 20;
    //    int.TryParse(controller.Request.Cookies[controller.RouteData.Values["controller"] + "_limit"] != null ?
    //            controller.Request.Cookies[controller.RouteData.Values["controller"] + "_limit"].Value :
    //            ConfigurationManager.AppSettings["ItemsPerPage"], out limit);
    //    System.Web.Routing.RouteValueDictionary query = new System.Web.Routing.RouteValueDictionary();
    //    foreach (var key in controller.HttpContext.Request.QueryString.AllKeys)
    //    {
    //        query.Add(key, controller.HttpContext.Request.QueryString[key]);
    //    }		
    //    return new PageInfo(limit, page, 0, new Func<int,string>(delegate (int i){
    //        query["page"] = i;
    //        return controller.Url.Action("Index", query);
    //    }));
    //}

    //public static PageInfo CalcBeginEnd(this Controller controller, string actionName, int page)
    //{
    //    System.Web.Routing.RouteValueDictionary query = new System.Web.Routing.RouteValueDictionary();
    //    foreach (var key in controller.HttpContext.Request.QueryString.AllKeys)
    //    {
    //        query.Add(key, controller.HttpContext.Request.QueryString[key]);
    //    }
    //    return new PageInfo(
    //        Convert.ToInt32(
    //            controller.Request.Cookies[controller.RouteData.Values["controller"] + "_limit"] != null ?
    //            controller.Request.Cookies[controller.RouteData.Values["controller"] + "_limit"].Value :
    //            ConfigurationManager.AppSettings["ItemsPerPage"]
    //        ), page, 0, new Func<int, string>(delegate(int i) {
    //        query["page"] = i;
    //        return controller.Url.Action(actionName, query);
    //    }));
    //}


    ///// <summary>
    ///// Chuyển List Item thành SelectList với valueField được định nghĩa trong model (SelectValueFieldAttribute, KeyAttribute), nếu không thấy sẽ lấy trường "Id" hoặc "Key" nếu có. TextField được định nghĩa (SelectTextFieldAttribute), nếu không có sẽ lấy trường "Name" hoặc "Value" nếu có.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="List"></param>
    ///// <param name="selectedValue"></param>
    ///// <returns></returns>
    //public static SelectList ToSelectList<T>(this IEnumerable<T> List, object selectedValue = null) {
    //    if (List == null)
    //    {
    //        return new SelectList(new List<string>());
    //    }
    //    string valueField = null;
    //    string textField = null;
    //    System.Reflection.PropertyInfo[] listPro = typeof(T).GetProperties();
    //    bool hasTextField = false;
    //    bool hasName = false;
    //    bool hasKeyAttribute = false;
    //    bool hasID = false;
    //    bool hasValueField = false;

    //    for (var i = 0; i < listPro.Length; i++) {

    //        System.Reflection.PropertyInfo pro = listPro[i];
    //        if (pro.GetCustomAttributes(typeof(SelectValueFieldAttribute), true).Length != 0) {
    //            valueField = pro.Name;
    //            hasValueField = true;
    //            if (hasTextField) break;
    //        }
    //        if (!hasValueField && pro.GetCustomAttributes(typeof(KeyAttribute), true).Length != 0) {
    //            valueField = pro.Name;
    //            hasKeyAttribute = true;
    //        }
    //        if (!hasValueField && !hasKeyAttribute && pro.Name.ToUpper() == "ID") {
    //            valueField = pro.Name;
    //            hasID = true;
    //        }
    //        if (!hasValueField && !hasKeyAttribute && !hasID && pro.Name.ToUpper() == "KEY") {
    //            valueField = pro.Name;
    //        }

    //        if (pro.GetCustomAttributes(typeof(SelectTextFieldAttribute), true).Length != 0) {
    //            hasTextField = true;
    //            textField = pro.Name;
    //            if (hasValueField) break;
    //        }
    //        if (!hasTextField && pro.Name.ToUpper() == "NAME") {
    //            hasName = true;
    //            textField = pro.Name;
    //        }
    //        if (!hasTextField && !hasName && pro.Name.ToUpper() == "VALUE") {
    //            textField = pro.Name;
    //        }
    //    }
    //    if (valueField == null && textField == null) {
    //        return new SelectList(List, selectedValue);
    //    } else {
    //        if (valueField == null) valueField = textField;
    //        if (textField == null) textField = valueField;
    //        return new SelectList(List, valueField, textField, selectedValue);
    //    }
    //}
	public static IDictionary<string, object> MergerAttribute(params object[] items) {
		Dictionary<string, object> kq = new Dictionary<string, object>();
		for (var i = 0; i < items.Length; i++) {
			object item = items[i];
			if (item == null)
				continue;
			Type type = item.GetType();
			if (item is System.Collections.IDictionary) {
				System.Collections.IDictionary dic = (System.Collections.IDictionary)item;
				foreach (var key in dic.Keys) {
					kq[key.ToString()] = dic[key];
				}
			} else {
				System.Reflection.PropertyInfo[] pros = type.GetProperties();
				for (var j = 0; j < pros.Length; j++) {
					System.Reflection.PropertyInfo pro = pros[j];
					var value = pro.GetValue(item, null);
					kq[pro.Name.Replace('_', '-')] = value;
				}
			}
		}
		return kq;
	}

	/// <summary>
	/// Loại bỏ các phần trùng nhau trong list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns>Return List phần tử không trùng</returns>
	public static IEnumerable<T> RemoveDuplicate<T>(this IEnumerable<T> list)
	{
		Dictionary<T, bool> dic = new Dictionary<T, bool>();
		foreach (var item in list)
		{
			if (item != null)
			{
				dic[item] = true;
			}
		}
		return dic.Keys.ToList();
	}
	/// <summary>
	///  Loại bỏ các phần trùng nhau trong list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="comparer">Cách so sách object bằng nhau</param>
	/// <returns>Return List phần tử không trùng</returns>
	public static IEnumerable<T> RemoveDuplicate<T>(this IEnumerable<T> list, IEqualityComparer<T> comparer)
	{
		Dictionary<T, bool> dic = new Dictionary<T, bool>(comparer);
		foreach (var item in list)
		{
			dic[item] = true;
		}
		return dic.Keys.ToList();
	}

	public static string JsonSelectField<T>(this T item, Func<T, dynamic> SelectField)
	{
		return System.Web.Helpers.Json.Encode(SelectField(item));
	}
	/// <summary>
	/// Ứng với kiểu ngày tháng khi encode json sẽ thành "\/Date(1350457433697)\/" (điều này làm javascript không hiểu). 	
	/// </summary>
	/// <param name="jsonText"></param>
	/// <returns>trả về các kiểu ngày tháng thành new Date(1350457433697) </returns>
	public static string FixJsonDate(this string jsonText)
	{
		Regex regex = new Regex("\"\\\\/(Date\\(\\d+\\))\\\\/\"");
		return regex.Replace(jsonText, new MatchEvaluator(delegate(Match match)
			{
				if (match.Groups.Count > 1)
				{
					return "new " + match.Groups[1].Value;
				}
				return match.Value;
			}));	
	}

	public static T Copy<T>(this T item) where T : new()
	{
		T kq = new T();
		var list = typeof(T).GetProperties();
		for (var i = 0; i < list.Length; i++)
		{
			var pro = list[i];
			if (pro.CanWrite && pro.CanRead)
			{
				pro.SetValue(kq, pro.GetValue(item, null), null);
			}
		}		
		return kq;
	}
	public static T Copy<T, TSource>(this TSource item) where T : TSource, new()
	{
		T kq = new T();
		var list = typeof(TSource).GetProperties();
		for (var i = 0; i < list.Length; i++)
		{
			var pro = list[i];
			if (pro.CanWrite && pro.CanRead)
			{
				pro.SetValue(kq, pro.GetValue(item, null), null);
			}
		}
		return kq;
	}

	public static Boolean IsDifference<T>(this T sourceItem, T targetItem, params string[] properties)
	{
		var listProperties = typeof(T).GetProperties();
		for (var i = 0; i < listProperties.Length; i++)
		{
			var pro = listProperties[i];
			if (properties != null && properties.Any())
			{
				if (Array.IndexOf(properties, pro.Name) != -1)
				{
					if (!pro.PropertyType.IsArray)
					{
						if (CheckDifference(sourceItem, targetItem, pro))
						{
							return true;
						}
					}
					else
					{
						continue;
					}
				}
			}
			else
			{
				if (!pro.PropertyType.IsArray)
				{
					if (CheckDifference(sourceItem, targetItem, pro))
					{
						return true;
					}
				}
				else
				{
					continue;
				}
			}
		}
		return false;
	}
	private static Boolean CheckDifference<T>(this T sourceItem, T targetItem, System.Reflection.PropertyInfo proInfo)
	{
		switch (proInfo.PropertyType.Name)
		{
			case "String":
				if (String.IsNullOrWhiteSpace(Convert.ToString(proInfo.GetValue(sourceItem, null))) == true && String.IsNullOrWhiteSpace(Convert.ToString(proInfo.GetValue(targetItem, null))) == true)
				{
					return false;
				}
				else if (proInfo.GetValue(sourceItem, null) != null && proInfo.GetValue(targetItem, null) != null)
				{
					if (proInfo.GetValue(sourceItem, null).Equals(proInfo.GetValue(targetItem, null)))
					{
						return false;
					}
				}
				break;
			default:
				if (proInfo.GetValue(sourceItem, null) == null && proInfo.GetValue(sourceItem, null) == null)
				{
					return false;
				}
				else if (proInfo.GetValue(sourceItem, null) != null && proInfo.GetValue(targetItem, null) != null)
				{
					if (proInfo.GetValue(sourceItem, null).Equals(proInfo.GetValue(targetItem, null)))
					{
						return false;
					}
				}
				break;
		}
		return true;
	}


	public static void AutoMap(object src, object desc)
	{
		var list = src.GetType().GetProperties();
		var desType = desc.GetType();
		for (var i = 0; i < list.Length; i++)
		{
			
			var srcProperty = list[i];
			if (srcProperty.CanRead)
			{
				var descProperty = desType.GetProperty(srcProperty.Name, srcProperty.PropertyType);
				if (descProperty != null && descProperty.CanWrite)
				{
					descProperty.SetValue(desc, srcProperty.GetValue(src, null), null);
				}
			}
		}
	}
	public static string GetDescriptionField<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
	{
		string kq = "";
		DisplayAttribute attr = GetAttributeField<DisplayAttribute>(expression);
		if (attr != null)
		{
			kq = attr.GetDescription();
		}
		return kq;
	}

	public static T GetAttributeField<T>(LambdaExpression expression) where T : Attribute
	{
		if (expression.Body is MemberExpression)
		{
			MemberExpression me = (MemberExpression)expression.Body;
			var obj = me.Member.GetCustomAttributes(typeof(T), true);
			if (obj != null && obj.Length > 0)
			{
				return  (T)obj[0];
			}
		}
		return null;
	}
	#endregion
	public static DateTime EndDate(this DateTime date)
	{
		return date.Date.Add(new TimeSpan(23, 59, 0));
	}
	public static DateTime? EndDate(this DateTime? date)
	{
		if (date.HasValue)
		{
			return date.Value.Date.Add(new TimeSpan(23, 59, 0));
		}
		return null;
	}

	public static string CleanScript(string Contents)
	{
		Contents = HttpUtility.UrlDecode(Regex.Replace(Contents, "[\t\v\b\f\r\0]|%00|&#8238;", "", RegexOptions.Compiled));
		Contents = Regex.Replace(Contents, @"\-moz\-binding", " moz binding", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		Contents = Regex.Replace(Contents, @"(java|vb)script\:", "$1 script:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		Contents = Regex.Replace(Contents, @"<(html|head|title|link|body|frame|iframe|cross\-domain\-policy)", "&lt; $1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		Contents = Regex.Replace(Contents, @"on(blur|change|focus|reset|select|submit|abort|error|load|unload|click|dblclick|keydown|keypress|keyup|mousedown|mousemove|mouseout|mouseover|mouseup)", "on $1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		return Contents;
	}
	public static T GetValue<T>(object value, string fieldName)
	{
		if (value is System.Collections.IDictionary)
		{
			System.Collections.IDictionary v = (System.Collections.IDictionary)value;
			return (T)v[fieldName];
		}
		var property = value.GetType().GetProperty(fieldName);
		if (property == null)
		{
			return default(T);
		}
		return (T) property.GetValue(value, null);		
	}

	/// <summary>
	/// Cắt chuỗi dài vượt quá giới hạn
	/// </summary>
	/// <param name="text">Chuỗi cần cắt</param>
	/// <param name="maxLength">Độ dài nếu chuỗi vượt qua sẽ bị cắt</param>
	/// <param name="bCutEndString">Cắt chuỗi hiển thị: 
	///		- true: cắt hiển thị ... ở giữa
	///		- false: cắt hiển thị ... ở cuối
	/// </param>
	/// <param name="maxLengthShow">Độ dài chuỗi hiển thị sau khi cắt</param>
	/// <returns></returns>
	public static string CutStringOff(this string text, int maxLength, Boolean bCutEndString, int maxLengthShow)
	{
		if (text == null) return null;
		if (maxLength == 0) maxLength = 80;
		if (maxLengthShow == 0) maxLengthShow = 30;

		if (text.Length > maxLength)
		{
			if (bCutEndString)
			{ return string.Format("{0} ... {1}", text.Substring(0, maxLengthShow), text.Substring(text.Length - maxLengthShow, maxLengthShow)); }
			else
			{ return string.Format("{0} ...", text.Substring(0, maxLengthShow)); }
		}
		else
		{ return text; }
	}

	public static List<KeyValuePair<int, string>> GetSelectListByXml(string fileXmlPath)
	{
		XmlDocument xml = new XmlDocument();
		xml.Load(fileXmlPath);
		List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
		for (var i = 0; i < xml.DocumentElement.ChildNodes.Count; i++)
		{
			var item  = xml.DocumentElement.ChildNodes[i];
			var id = Convert.ToInt32(item.Attributes["id"].Value);
			list.Add(new KeyValuePair<int, string>(id, item.InnerText));
		}
		return list;
	}

	public static T CreateInstance<T>()
	{
		var type = typeof(T);
		if (type == typeof(string))
		{
			return (T)(object)string.Empty;
		}
		return (T)FormatterServices.GetUninitializedObject(type);
	}

	public static T JsonParse<T>(string s) where T : new()
	{
		return Json.Decode<T>(s);
	}
	public static dynamic JsonParse(string s)
	{
		return Json.Decode(s);
	}

	/// <summary>
	/// ma hoa chuoi input
	/// </summary>
	/// <param name="strInput">du lieu can duoc ma hoa</param>
	/// <param name="strKey">chieu dai bat buoc 16 ki tu</param>
	/// <returns></returns>
	public static string Encript(string strInput, string strKey)
	{
		byte[] key = { };
		byte[] IV = { 0x96, 0x85, 0x74, 0x63, 0x52, 0x41, 0x98, 0x65 };
		try
		{
			key = Encoding.UTF8.GetBytes(strKey);
			using (DESCryptoServiceProvider oDESCryptTo = new DESCryptoServiceProvider())
			{
				byte[] inputByteArray = Encoding.UTF8.GetBytes(strInput);
				MemoryStream oMemoryStream = new MemoryStream();
				CryptoStream oCryptoStream = new CryptoStream(oMemoryStream,
				oDESCryptTo.CreateEncryptor(key, IV), CryptoStreamMode.Write);
				oCryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
				oCryptoStream.FlushFinalBlock();
				return Convert.ToBase64String(oMemoryStream.ToArray());
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	/// <summary>
	/// giai ma chuoi input
	/// </summary>
	/// <param name="strInput">du lieu can duoc giai ma</param>
	/// <param name="strKey">chieu dai bat buoc 16 ki tu</param>
	/// <returns></returns>
	public static string Decript(string strInput, string strKey)
	{
		byte[] key = { };
		byte[] IV = { 0x96, 0x85, 0x74, 0x63, 0x52, 0x41, 0x98, 0x65 };
		strInput = strInput.Replace(' ', '+');
		byte[] inputByteArray = new byte[strInput.Length];
		try
		{
			key = Encoding.UTF8.GetBytes(strKey);
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			inputByteArray = Convert.FromBase64String(strInput);
			MemoryStream oMemoryStream = new MemoryStream();
			CryptoStream oCryptoStream = new CryptoStream(oMemoryStream, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
			oCryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
			oCryptoStream.FlushFinalBlock();
			return Encoding.UTF8.GetString(oMemoryStream.ToArray());
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
}

#region Hung
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SelectTextFieldAttribute : Attribute {
}
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SelectValueFieldAttribute : Attribute {
}

#endregion

/// <summary>
/// Pagination class.
/// </summary>
public class PageInfo {
	public PageInfo() {
		Url = new Func<int, string>(i => "");
	}
	public PageInfo(int limit, int page)
		: this(limit, page, 0, false, false, u=> string.Empty)
	{
	}
	public PageInfo(int limit, int page, int count, Func<int, string> url)
		: this(limit, page, count, false, false, url) {
	}
	public PageInfo(int limit, int page, int count, bool hlb, bool hjb, Func<int, string> url) {
		ItemsPerPage = limit; CurrentPage = page; TotalItems = count; HideLimitBox = hlb; HideJumpBox = hjb; Url = url;
	}
	public int Begin { get { return (_page - 1) * _limit + 1; } }
	public int End { get { return _page * _limit; } }
	public int ItemsPerPage {
		get { return _limit; }
		set { _limit = (value < 5 ? 5 : (value > 200 ? 200 : value)); }
	}
	public int CurrentPage {
		get { return _page; }
		set { _page = value < 1 ? 1 : value; }
	}
	public int TotalItems { get; set; }
	public bool HideLimitBox { get; set; }
	public bool HideJumpBox { get; set; }

	public int TotalPage
	{
		get
		{
			return this.ItemsPerPage > 0 ? (int)Math.Ceiling((decimal)this.TotalItems / this.ItemsPerPage) : 0;
		}
	}
	public Func<int, string> Url { get; set; }
	public readonly int Leave = 3;
	private int _limit, _page;
}

