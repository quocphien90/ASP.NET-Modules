using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

namespace OgilvyOne.MSSQL
{
	/// <summary>
	/// Summary description for SqlDatabaseProvider
	/// </summary>
	public class MSSQLDbProvider : DbProvider
	{
        
        #region Constructor
		/// <summary>
		/// Create Sql DataBase Driver with connection string
		/// </summary>
		/// <param name="connectionString"></param>
		public MSSQLDbProvider(String connectionString)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			this.dbCommand = conn.CreateCommand();
			this.dataAdapter = new SqlDataAdapter();
			this.HandleErrors = false;
			this.InTransaction = false;
			this.ErrorMessage = new System.Text.StringBuilder();
		}

		/// <summary>
		/// Create Sql DataBase Driver with SqlConnection has connection string
		/// </summary>
		/// <param name="connection"></param>
		public MSSQLDbProvider(SqlConnection connection)
		{
			this.dbCommand = connection.CreateCommand();
			this.dataAdapter = new SqlDataAdapter();
			this.HandleErrors = false;
			this.InTransaction = false;
			this.ErrorMessage = new System.Text.StringBuilder();
		}
       
		/// <summary>
		/// Create Sql DataBase Driver with connection string, And handle errors. 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="handleErrors">true : handle errors, false : throw errors</param>
		public MSSQLDbProvider(String connectionString, bool handleErrors)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			this.dbCommand = conn.CreateCommand();
			this.dbCommand.Connection = conn;
			this.dataAdapter = new SqlDataAdapter();
			this.HandleErrors = handleErrors;
			this.InTransaction = false;
			this.ErrorMessage = new System.Text.StringBuilder();
		}
        
		/// <summary>
		/// Get SqlCommand of this DatabaseProvider
		/// </summary>
		public SqlCommand Command
		{
			get { return dbCommand; }
		}
		#endregion

		#region OverRide Properties
		private SqlCommand dbCommand;
		protected override IDbCommand DbCommand
		{
			get { return dbCommand; }
		}

		private SqlDataAdapter dataAdapter;
		protected override IDbDataAdapter DataAdapter
		{
			get { return dataAdapter; }
		}

		public override IDbConnection Connection
		{
			get { return dbCommand.Connection; }
		}
		#endregion

		#region Parameter
		/// <summary>
		/// Add Parameter into command with name, value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns>SqlParameter</returns>
		public new SqlParameter AddParameter(string name, object value)
		{
			return base.AddParameter(name, value) as SqlParameter;
		}

		/// <summary>
		/// Add Parameter into command with name, value, type and direction
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="direction"></param>
		/// <returns>SqlParameter</returns>
		public SqlParameter AddParameter(string name, object value, SqlDbType sqlDbType, ParameterDirection parameterDirection)
		{
			SqlParameter para = new SqlParameter(name, value);
			para.SqlDbType = sqlDbType;
			para.Direction = parameterDirection;
			dbCommand.Parameters.Add(para);
			return para;
		}

		/// <summary>
		/// Add Parameter into command
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns>Parameter</returns>
		public SqlParameter AddParameter(SqlParameter parameter)
		{
			return base.AddParameter(parameter) as SqlParameter;
		}

		/// <summary>
		/// Add Parameter into command
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns>Parameter</returns>
		private new IDbDataParameter AddParameter(IDbDataParameter parameter)
		{
			return base.AddParameter(parameter);
		}		
		#endregion

		#region ExecuteDataTable OverLoad
		/// <summary>
		/// Executes the query, and returns DataTable
		/// </summary>
		/// <returns></returns>
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public DataTable ExecuteDataTable()
		{
			DataTable dta = new DataTable();
			try
			{
				dataAdapter.SelectCommand = dbCommand;
				dataAdapter.Fill(dta);

			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : ExecuteDataTable ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			return dta;
		}

		/// <summary>
		/// Executes the query, and returns DataTable
		/// </summary>
		/// <param name="commandText">specified CommandText</param>
		/// <param name="type">CommandType of CommandText</param>
		/// <param name="parameters">specified ParameterCollection </param>
		/// <returns></returns>
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public DataTable ExecuteDataTable(string commandText, CommandType type, SqlParameterCollection parameters)
		{
			DataTable dta = new DataTable();
			try
			{
				this.SetCommandText(commandText, type);
				foreach (SqlParameter para in parameters)
				{
					dbCommand.Parameters.Add(para);
				}
				dataAdapter.SelectCommand = dbCommand;
				dataAdapter.Fill(dta);

			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : ExecuteDataTable ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			return dta;
		}

		/// <summary>
		/// Executes the query, and returns DataTable
		/// </summary>
		/// <param name="commandText">specified CommandText</param>
		/// <param name="type">CommandType of CommandText</param>
		/// <returns></returns>
		public DataTable ExecuteDataTable(string commandText, CommandType type)
		{
			DataTable dta = new DataTable();
			try
			{
				dbCommand.CommandText = commandText;
				dbCommand.CommandType = type;
				dataAdapter.SelectCommand = dbCommand;
				dataAdapter.Fill(dta);

			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : ExecuteDataTable ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			return dta;
		}
		#endregion

		#region Fill OverLoad
		/// <summary>
		/// Adds or refreshes rows in the DataTable 
		/// </summary>
		/// <param name="table">The DataTable to use for table mapping.</param>
		/// <returns>The number of rows successfully added to or refreshed in the DataTable</returns>
		public int Fill(DataTable table)
		{
			int kq = 0;
			try
			{
				dataAdapter.SelectCommand = dbCommand;
				kq = dataAdapter.Fill(table);
			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : Fill(DataTable) ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			return kq;
		}

		/// <summary>
		/// Adds or refreshes rows in DataSet to match those in the data source using the DataSet and DataTable names
		/// </summary>
		/// <param name="dataSet">A DataSet to fill with records and, if necessary, schema</param>
		/// <param name="srcTable">The name of the source table to use for table mapping</param>
		/// <returns>The number of rows successfully added to or refreshed in the DataSet</returns>
		public int Fill(DataSet dataSet, string srcTable)
		{
			int kq = 0;
			try
			{
				dataAdapter.SelectCommand = dbCommand;
				kq = dataAdapter.Fill(dataSet, srcTable);
			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : Fill(DataTable) ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			return kq;
		}

		/// <summary>
		/// Adds or refreshes rows in a specified range in the DataSet to match those in the data source using the DataSet and DataTable names
		/// </summary>
		/// <param name="dataSet">A DataSet to fill with records and, if necessary, schema</param>
		/// <param name="startRecord">The zero-based record number to start with. </param>
		/// <param name="maxRecord">The maximum number of records to retrieve</param>
		/// <param name="srcTable">The name of the source table to use for table mapping</param>
		/// <returns>The number of rows successfully added to or refreshed in the DataSet</returns>
		public int Fill(DataSet dataSet, int startRecord, int maxRecord, string srcTable)
		{
			int kq = 0;
			try
			{
				dataAdapter.SelectCommand = dbCommand;
				kq = dataAdapter.Fill(dataSet, startRecord, maxRecord, srcTable);
			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : Fill(DataTable) ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			return kq;
		}
		#endregion

		#region ExecuteReader OverLoad 
		/// <summary>
		/// Execute Command 
		/// </summary>
		/// <returns>Return one Reader. Should close this reader when not use</returns>
		public new SqlDataReader ExecuteReader()
		{
			return base.ExecuteReader() as SqlDataReader;
		}

		/// <summary>
		/// Execute Command with specified CommandText
		/// </summary>
		/// <param name="commandText">specified CommandText</param>
		/// <param name="type">CommandType of CommandText</param>
		/// <returns>Return one Reader. Should close this reader when not use</returns>
		public new SqlDataReader ExecuteReader(string commandText, CommandType type)
		{
			return base.ExecuteReader(commandText, type) as SqlDataReader;
		}

		/// <summary>
		/// Execute Command with specified CommandText and Parameters
		/// </summary>
		/// <param name="commandText">specified CommandText</param>
		/// <param name="type">CommandType of CommandText</param>
		/// <param name="parameters">specified ParameterCollection </param>
		/// <returns>Return one Reader. Should close this reader when not use</returns>
		public SqlDataReader ExecuteReader(string commandText, CommandType type, SqlParameterCollection parameters)
		{
			return base.ExecuteReader(commandText, type, parameters) as SqlDataReader;
		}

		#endregion

		public void Fill<T>(ICollection<T> listKq)
		{
			SqlDataReader reader = null;
			try
			{
				this.Open();
				reader = this.Command.ExecuteReader();
				while (reader.Read())
				{

					listKq.Add((T)GetObjectFormReader(reader, typeof(T)));
				}
			}
			catch (Exception ex)
			{
				if (this.InTransaction)
				{
					this.RollBack();
				}
				if (HandleErrors)
				{
					ErrorMessage.Append("Error : Fill object ");
					ErrorMessage.Append(ex.Message);
					ErrorMessage.Append("\n");
				}
				else
				{
					throw;
				}
			}
			finally
			{
				if (reader != null) reader.Close();
				if (!this.InTransaction) this.Close();
			}
		}
	}

	
}