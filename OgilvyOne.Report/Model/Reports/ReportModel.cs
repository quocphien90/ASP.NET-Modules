using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OgilvyOne.Report.Model.Reports
{

    public class ReportProperty
    {
        public long ID { get; set; }
        public long QueryID { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }

        public ReportPropertyDataType DataType { get; set; }
        public string DataPreValue { get; set; }

        public int SortOrder { get; set; }
        public bool Mandatory { get; set; }
    }

    public enum ReportPropertyDataType
    {
        Int = 0,
        Date = 1,
        String = 2,
        Select = 3,
        Checkbox = 4
    }

    public class ReportQuery
    {
        public ReportQuery()
        {
            this.ID = 0;
            this.Status = ReportQueryStatus.UnApprove;
            this.CreatedDate = this.ModifiedDate = DateTime.Now;
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public string QueryCount { get; set; }
        public string QueryData { get; set; }
        public ReportQueryStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public enum ReportQueryStatus
    {
        UnApprove = 0,
        Approve = 1
    }

    public class ReportQueryCondition
    {
        public string Name { get; set; }
        public ReportQueryStatus? Status { get; set; }
    }

}