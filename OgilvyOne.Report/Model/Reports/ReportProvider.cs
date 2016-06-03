using OgilvyOne.Report.DataHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.DataLayer;

namespace OgilvyOne.Report.Model.Reports
{
    public class ReportProvider
    {
        #region private
        private static ISqlHelper SqlHelper
        {
            get { return Application.SqlHelper; }
        }
        #endregion

        #region Report Query
        public ReportQuery InsertReportQuery(ReportQuery data)
        {
            data.ID = SqlHelper.ExecuteScalar<long>("INSERT INTO [rpQueries] ([Name],[QueryCount],[QueryData],[Status],[CreatedDate],[ModifiedDate],[CreatedBy],[ModifiedBy]) OUTPUT Inserted.ID VALUES (@Name,@QueryCount,@QueryData,@Status,@CreatedDate,@ModifiedDate,@CreatedBy,@ModifiedBy)",
                new IParameter[] { 
					SqlHelper.Parameter("Name", data.Name),
					SqlHelper.Parameter("QueryCount", data.QueryCount),
					SqlHelper.Parameter("QueryData", data.QueryData),
					SqlHelper.Parameter("Status", data.Status),
					SqlHelper.Parameter("CreatedDate", data.CreatedDate),
					SqlHelper.Parameter("ModifiedDate", data.ModifiedDate),
					SqlHelper.Parameter("CreatedBy", data.CreatedBy),
					SqlHelper.Parameter("ModifiedBy", data.ModifiedBy)
			});

            return data;
        }
        public void UpdateReportQuery(ReportQuery data)
        {
            SqlHelper.ExecuteNonQuery("UPDATE [rpQueries] SET [Name]=@Name,[QueryCount]=@QueryCount,[QueryData]=@QueryData,[Status]=@Status,[ModifiedDate]=@ModifiedDate,[ModifiedBy]=@ModifiedBy WHERE [ID]=@ID",
                new IParameter[]{
					SqlHelper.Parameter("ID", data.ID),
					SqlHelper.Parameter("Name", data.Name),
					SqlHelper.Parameter("QueryCount", data.QueryCount),
					SqlHelper.Parameter("QueryData", data.QueryData),
					SqlHelper.Parameter("Status", data.Status),
					SqlHelper.Parameter("ModifiedDate", data.ModifiedDate),
					SqlHelper.Parameter("ModifiedBy", data.ModifiedBy)
			});
        }
        public void UpdateReportQueryStatus(long ID, ReportQueryStatus Status)
        {
            SqlHelper.ExecuteNonQuery("UPDATE [rpQueries] SET [Status]=@Status WHERE [ID]=@ID",
                new IParameter[]{
					SqlHelper.Parameter("ID", ID),
					SqlHelper.Parameter("Status", Status)
			});
        }
        public void DeleteReportQuery(long ID)
        {
            SqlHelper.ExecuteNonQuery("DELETE FROM [rpQueries] WHERE [ID]=@ID",
                new IParameter[]{
					SqlHelper.Parameter("ID", ID)
			});
        }

        public ReportQuery SelectReportQueryByID(long ID)
        {
            using (var reader = SqlHelper.ExecuteReader("SELECT * FROM [rpQueries] WHERE [ID]=@ID", new IParameter[] { SqlHelper.Parameter("ID", ID) }))
            {
                if (reader.Read())
                {
                    ReportQuery result = new ReportQuery();
                    DataMapper.Mapper<ReportQuery>(reader, result);
                    return result;
                }
            }
            return null;
        }
        public List<ReportQuery> SelectReportQuery(ReportQueryCondition condition, int begin, int end, out int total)
        {
            List<ReportQuery> listResult = new List<ReportQuery>();
            List<IParameter> listParameter = new List<IParameter>();

            listParameter.Add(SqlHelper.Parameter("Name", condition.Name));
            listParameter.Add(SqlHelper.Parameter("Status", condition.Status));

            total = SqlHelper.ExecuteScalar<int>("SELECT COUNT(1)[Total] FROM [rpQueries] WHERE (LEN(@Name) = 0 OR @Name IS NULL OR [Name] LIKE @Name + '%') AND [Status] = ISNULL(@Status,[Status])", listParameter.ToArray());

            listParameter.Add(SqlHelper.Parameter("Begin", begin));
            listParameter.Add(SqlHelper.Parameter("End", end));

            using (var reader = SqlHelper.ExecuteReader(@"SELECT * FROM 
				(SELECT *, ROW_NUMBER() OVER(ORDER BY [ID] DESC) AS [RowNum] FROM [rpQueries] WHERE (LEN(@Name) = 0 OR @Name IS NULL OR [Name] LIKE @Name + '%') AND [Status] = ISNULL(@Status,[Status])) AS [T]
				WHERE [RowNum] BETWEEN @Begin AND @End", listParameter.ToArray()))
            {
                while (reader.Read())
                {
                    ReportQuery result = new ReportQuery();
                    DataMapper.Mapper<ReportQuery>(reader, result);
                    listResult.Add(result);
                }
            }
            return listResult;
        }
        #endregion

        #region Report Properties
        public void InsertReportProperty(ReportProperty data)
        {
            SqlHelper.ExecuteNonQuery("INSERT INTO [rpQueryProperties] ([QueryID],[Alias],[Name],[Description],[DataType],[DataPreValue],[SortOrder],[Mandatory]) VALUES (@QueryID,@Alias,@Name,@Description,@DataType,@DataPreValue,@SortOrder,@Mandatory)",
                new IParameter[]{
					SqlHelper.Parameter("QueryID", data.QueryID),
					SqlHelper.Parameter("Alias", data.Alias),
					SqlHelper.Parameter("Name", data.Name),
					SqlHelper.Parameter("Description", data.Description),
					SqlHelper.Parameter("DataType", data.DataType),
					SqlHelper.Parameter("DataPreValue", data.DataPreValue),
					SqlHelper.Parameter("SortOrder", data.SortOrder),
					SqlHelper.Parameter("Mandatory", data.Mandatory)
			});
        }
        public void UpdateReportProperty(ReportProperty data)
        {
            SqlHelper.ExecuteNonQuery("UPDATE [rpQueryProperties] SET [QueryID]=@QueryID,[Alias]=@Alias,[Name]=@Name,[Description]=@Description,[DataType]=@DataType,[DataPreValue]=@DataPreValue,[SortOrder]=@SortOrder,[Mandatory]=@Mandatory WHERE [ID]=@ID",
                new IParameter[]{
					SqlHelper.Parameter("ID", data.ID),
					SqlHelper.Parameter("QueryID", data.QueryID),
					SqlHelper.Parameter("Alias", data.Alias),
					SqlHelper.Parameter("Name", data.Name),
					SqlHelper.Parameter("Description", data.Description),
					SqlHelper.Parameter("DataType", data.DataType),
					SqlHelper.Parameter("DataPreValue", data.DataPreValue),
					SqlHelper.Parameter("SortOrder", data.SortOrder),
					SqlHelper.Parameter("Mandatory", data.Mandatory)
			});
        }
        public void DeleteReportProperty(long ID)
        {
            SqlHelper.ExecuteNonQuery("DELETE FROM [rpQueryProperties]WHERE [ID]=@ID",
                new IParameter[]{
					SqlHelper.Parameter("ID", ID)
			});
        }
        public void DeleteReportPropertyByReportQuery(long QueryID)
        {
            SqlHelper.ExecuteNonQuery("DELETE FROM [rpQueryProperties]WHERE [QueryID]=@QueryID",
                new IParameter[]{
					SqlHelper.Parameter("QueryID", QueryID)
			});
        }
        public void ChangeSortOrderPropertiy(long QueryID, int[] listPropertiesID)
        {
            if (!listPropertiesID.Any())
                return;

            StringBuilder sbSql = new StringBuilder();
            List<IParameter> param = new List<IParameter>();

            param.Clear();
            sbSql.Append("DECLARE @tmp TABLE(ID int, SortOrder int);");
            sbSql.AppendLine();

            string sqlInsert = "INSERT INTO @tmp (ID, SortOrder) VALUES(@ID{0},@Order{0});";
            for (int i = 0; i < listPropertiesID.Length; i++)
            {
                param.Add(SqlHelper.Parameter(string.Format("ID{0}", i), listPropertiesID[i]));
                param.Add(SqlHelper.Parameter(string.Format("Order{0}", i), i));
                sbSql.AppendFormat(sqlInsert, i);
                sbSql.AppendLine();
            }

            sbSql.AppendLine("UPDATE [rpQueryProperties] SET [SortOrder]=b.[SortOrder] FROM [rpQueryProperties] a INNER JOIN @tmp b ON a.[ID]=b.[ID] WHERE [QueryID]=@QueryID");
            param.Add(SqlHelper.Parameter("QueryID", QueryID));
            sbSql.AppendLine();

            SqlHelper.ExecuteNonQuery(sbSql.ToString(), param.ToArray());
        }

        public ReportProperty SelectReportProperty(long ID)
        {
            using (var reader = SqlHelper.ExecuteReader("SELECT * FROM [rpQueryProperties] WHERE [ID]=@ID", new IParameter[] { SqlHelper.Parameter("ID", ID) }))
            {
                if (reader.Read())
                {
                    ReportProperty result = new ReportProperty();
                    DataMapper.Mapper<ReportProperty>(reader, result);
                    return result;
                }
            }
            return null;
        }
        public List<ReportProperty> SelectReportPropertyByReportQuery(long QueryID)
        {
            List<ReportProperty> listResult = new List<ReportProperty>();

            using (var reader = SqlHelper.ExecuteReader("SELECT * FROM [rpQueryProperties] WHERE [QueryID]=@QueryID ORDER BY [SortOrder],[ID]", new IParameter[] { SqlHelper.Parameter("QueryID", QueryID) }))
            {
                while (reader.Read())
                {
                    ReportProperty result = new ReportProperty();
                    DataMapper.Mapper<ReportProperty>(reader, result);
                    listResult.Add(result);
                }
            }

            return listResult;
        }
        #endregion
    }

}