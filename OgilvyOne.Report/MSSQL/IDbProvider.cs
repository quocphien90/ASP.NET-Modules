using System;
using System.Collections.Generic;
using System.Data;

namespace OgilvyOne.MSSQL {
	public interface IDbProvider {
		IDbDataParameter AddParameter(IDbDataParameter parameter);
		IDbDataParameter AddParameter(string name, object value);
		IDbDataParameter AddParameter(string name, object value, ParameterDirection direction);
		IDbDataParameter AddParameter(string name, object value, DbType type, ParameterDirection direction);
		/// <summary>
		/// Add Parameter into command with name, value and direction (with prarmeter type is string)
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		IDbDataParameter AddParameter(string name, string value, ParameterDirection direction);
		IDbDataParameter AddParameter(string name, string value, int size,  ParameterDirection direction);
		void AddParameterMapObject(object paramaterObject);
		IDbDataParameter this[string parameterName] { get; }
		void BeginTransaction();
		void BeginTransaction(IsolationLevel lv);
		string ClearError();
		void ClearParameter();
		void Close();
		void Commit();
		IDbConnection Connection { get; }
		string CommandText { get; set; }
		CommandType CommandType { get; set; }
		void Dispose();
		DataSet ExecuteDatSet();
		DataSet ExecuteDatSet(string commandText, CommandType type);
		DataSet ExecuteDatSet(string commandText, CommandType type, IDataParameterCollection parameters);
		Dictionary<TKey, TTarget> ExecuteDictionaryObject<TKey, TTarget>()
			where TKey : new()
			where TTarget : new();
		Dictionary<TKey, TTarget> ExecuteDictionaryObject<TKey, TTarget>(string keyField) where TTarget : new();
		Dictionary<string, TTarget> ExecuteDictionaryObjectByStringKey<TTarget>(params string[] keyFields) where TTarget : new();
		List<TTarget> ExecuteList<TTarget>(string fieldName);
		List<TTarget> ExecuteListObject<TTarget>() where TTarget : new();
		IList<TTarget> ExecuteListObject<TTarget>(string commandText, CommandType type, params IDbDataParameter[] parameters) where TTarget : new();
		int ExecuteNonQuery();
		int ExecuteNonQuery(string commandText, CommandType type);
		int ExecuteNonQuery(string commandText, CommandType type, IDataParameterCollection parameters);
		TTarget ExecuteObject<TTarget>() where TTarget : new();
		TTarget ExecuteObject<TTarget>(string commandText, CommandType type, params IDbDataParameter[] parameters) where TTarget : new();
		IDataReader ExecuteReader();
		IDataReader ExecuteReader(string commandText, CommandType type);
		IDataReader ExecuteReader(string commandText, CommandType type, params IDbDataParameter[] parameters);
		IDataReader ExecuteReader(string commandText, CommandType type, IDataParameterCollection parameters);
		object ExecuteScalar();
		object ExecuteScalar(string commandText, CommandType type);
		object ExecuteScalar(string commandText, CommandType type, IDataParameterCollection parameters);
		int Fill(DataSet dataSet);
		void FillDictionaryObject<TKey, TTarget>(IDictionary<TKey, TTarget> list, string keyField) where TTarget : new();
		string GetError();
		void Open();
		void RollBack();
		void SetCommandText(string commandText, CommandType type);
	}
}